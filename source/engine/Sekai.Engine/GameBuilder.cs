// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sekai.Engine.Assets;
using Sekai.Engine.Effects;
using Sekai.Engine.Resources;
using Sekai.Framework;
using Sekai.Framework.Audio;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;
using Sekai.Framework.Logging;
using Sekai.Framework.Storage;
using Sekai.Framework.Windowing;

namespace Sekai.Engine;

public sealed class GameBuilder<T>
    where T : Game, new()
{
    private readonly T game;
    private readonly GameOptions options;
    private IWindow? window;
    private IAudioContext? audio;
    private IInputContext? input;
    private IGraphicsDevice? graphics;
    private readonly List<Action<T>> postBuildActions = new();

    internal GameBuilder(T game, GameOptions? options = null)
    {
        this.game = game;
        this.options = options ?? new();
    }

    public GameBuilder<T> UseAudio<U>()
        where U : IAudioContext, new()
    {
        audio = new U();
        return this;
    }

    /// <summary>
    /// Uses the input context of a given type.
    /// </summary>
    public GameBuilder<T> UseInput<U>()
        where U : IInputContext, new()
    {
        input = new U();
        return this;
    }

    /// <summary>
    /// Uses the window of a given type.
    /// </summary>
    public GameBuilder<T> UseWindow<U>()
        where U : IWindow, new()
    {
        window = new U();
        return this;
    }

    /// <summary>
    /// Uses the graphics device of a given type.
    /// </summary>
    public GameBuilder<T> UseGraphics<U>()
        where U : IGraphicsDevice, new()
    {
        graphics = new U();
        return this;
    }

    /// <summary>
    /// Adds a build action.
    /// </summary>
    public GameBuilder<T> AddBuildAction(Action<T> action)
    {
        postBuildActions.Add(action);
        return this;
    }

    /// <summary>
    /// Builds the game.
    /// </summary>
    public T Build()
    {
        if (RuntimeInfo.IsDebug)
            Logger.OnMessageLogged += new LogListenerConsole();

        if (window == null)
            throw new InvalidOperationException();

        var storage = new VirtualStorage();

        storage.Mount("/engine", new AssemblyBackedStorage(ResourceAssembly.Assembly));
        storage.Mount("/game", new NativeStorage(AppDomain.CurrentDomain.BaseDirectory));

        var stream = storage.Open("/game/runtime.log");
        var writer = new LogListenerTextWriter(stream);
        Logger.OnMessageLogged += writer;

        game.OnExiting += writer.Dispose;

        game.Services.Cache(storage);

        var entry = Assembly.GetEntryAssembly()?.GetName();

        Logger.Log("----------------------------------------------------------");
        Logger.Log($"Logging for {Environment.UserName}");
        Logger.Log($"Running {entry?.Name ?? "Unknown"} {entry?.Version} on .NET {Environment.Version}");
        Logger.Log($"Environment: {RuntimeInfo.OS} ({Environment.OSVersion.VersionString})");
        Logger.Log("----------------------------------------------------------");

        window.Size = options.Size;
        window.Title = options.Title;
        window.Border = WindowBorder.Resizable;
        window.Visible = false;
        window.OnClose += game.Exit;

        game.Services.Cache<IView>(window);

        if (audio != null)
            game.Services.Cache(audio);

        if (input != null)
            game.Services.Cache(input);

        options.Graphics.Debug ??= RuntimeInfo.IsDebug;
        options.Graphics.GraphicsAPI ??= RuntimeInfo.OS.GetGraphicsAPI();

        if (graphics == null)
            throw new InvalidOperationException();

        graphics.Initialize(window, options.Graphics);
        Logger.Log($"{graphics.GraphicsAPI} Initialized");
        Logger.Log($"{graphics.GraphicsAPI} Device: {graphics.Name}");

        game.Services.Cache(graphics);

        var systems = new SystemCollection<GameSystem>();

        game.Services.Cache(systems);
        game.Services.Cache(systems.Register<SceneController>());

        var loader = new AssetLoader();
        loader.Register<Effect, EffectLoader>();
        loader.Register<ITexture, TextureLoader>();

        game.Services.Cache(loader);
        game.Services.Cache(options);

        foreach (var action in postBuildActions)
            action.Invoke(game);

        return game;
    }

    /// <summary>
    /// Builds then runs the game.
    /// </summary>
    public void Run()
    {
        Build().Run();
    }
}
