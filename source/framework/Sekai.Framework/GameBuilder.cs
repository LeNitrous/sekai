// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sekai.Framework.Audio;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;
using Sekai.Framework.Logging;
using Sekai.Framework.Scenes;
using Sekai.Framework.Services;
using Sekai.Framework.Storage;
using Sekai.Framework.Windowing;

namespace Sekai.Framework;

public sealed class GameBuilder<T>
    where T : Game, new()
{
    private readonly T game;
    private readonly GameOptions options;
    private IWindow? window;
    private IAudioContext? audio;
    private IInputContext? input;
    private IGraphicsContext? graphics;
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
        where U : IGraphicsContext, new()
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

        var storage = new VirtualStorage();
        storage.Mount("/", new NativeStorage(AppDomain.CurrentDomain.BaseDirectory));
        storage.Mount("/engine", new AssemblyBackedStorage(typeof(Game).Assembly, "Resources"));

        var stream = storage.Open("/runtime.log");
        var writer = new LogListenerTextWriter(stream);
        Logger.OnMessageLogged += writer;
        game.OnExiting += writer.Dispose;

        var entry = Assembly.GetEntryAssembly()?.GetName();

        Logger.Log("----------------------------------------------------------");
        Logger.Log($"Logging for {Environment.UserName}");
        Logger.Log($"Running {entry?.Name ?? "Unknown"} {entry?.Version} on .NET {Environment.Version}");
        Logger.Log($"Environment: {RuntimeInfo.OS} ({Environment.OSVersion.VersionString})");
        Logger.Log("----------------------------------------------------------");

        if (window != null)
        {
            window.Size = options.Size;
            window.Title = options.Title;
            window.Border = WindowBorder.Resizable;
            window.Visible = false;
            window.OnClose += game.Exit;
            game.Services.Register<IView>(window);
        }

        if (graphics != null)
        {
            if (window == null)
                throw new InvalidOperationException(@"A window is required when initializing a graphics context.");

            graphics.Initialize(window);
            game.Services.Register(graphics);
        }

        if (audio != null)
            game.Services.Register(audio);

        if (input != null)
            game.Services.Register(input);

        Logger.Log("Runtime Information");
        Logger.Log($"Input     : {input?.GetType().Name ?? "None"}");
        Logger.Log($"Audio     : {audio?.GetType().Name ?? "None"}");
        Logger.Log($"Graphics  : {graphics?.GetType().Name ?? "None"}");
        Logger.Log($"Windowing : {window?.GetType().Name ?? "None"}");

        game.Services.Register(storage);
        game.Services.Register(options);
        game.Services.Register<SceneController>();
        game.Services.Register<BehaviorService>();
        game.Services.Register<ComponentService>();

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
