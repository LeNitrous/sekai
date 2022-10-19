// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using Sekai.Engine.Assets;
using Sekai.Engine.Effects;
using Sekai.Engine.Graphics;
using Sekai.Engine.Resources;
using Sekai.Engine.Threading;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Storage;
using Sekai.Framework.Windowing;

namespace Sekai.Engine;

public sealed class GameBuilder<T>
    where T : Game, new()
{
    private readonly T game;
    private readonly GameOptions options;
    private IWindow window = null!;
    private IGraphicsDevice graphics = null!;
    private ThreadController threads = null!;
    private readonly VirtualStorage storage = new();
    private readonly List<Action<T>> postBuildActions = new();

    internal GameBuilder(T game, GameOptions? options = null)
    {
        this.game = game;
        this.options = options ?? new();
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
    /// Uses the graphics context of a given type.
    /// </summary>
    public GameBuilder<T> UseGraphics<U>()
        where U : IGraphicsDevice, new()
    {
        graphics = new U();
        return this;
    }

    public GameBuilder<T> AddPostBuildAction(Action<T> action)
    {
        postBuildActions.Add(action);
        return this;
    }

    public T Build()
    {
        if (RuntimeInfo.IsDebug)
            Logger.OnMessageLogged += new LogListenerConsole();

        window.Size = options.Size;
        window.Title = options.Title;
        window.Border = WindowBorder.Resizable;
        window.Visible = true;
        window.OnClose += game.Exit;

        options.Graphics.Debug ??= RuntimeInfo.IsDebug;
        options.Graphics.GraphicsAPI ??= getGraphicsAPIForPlatform();

        graphics.Initialize(window, options.Graphics);
        Logger.Log($"{graphics.GraphicsAPI} Initialized");
        Logger.Log($"{graphics.GraphicsAPI} Device: {graphics.Name}");

        storage.Mount("/engine", new AssemblyBackedStorage(ResourceAssembly.Assembly));
        storage.Mount("/engine/logs", new NativeStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")));
        storage.Mount("/engine/cache", new NativeStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache")));

        var systems = new SystemCollection<GameSystem>();
        systems.Register<SceneController>();

        game.Services.Cache<IView>(window);
        game.Services.Cache(window.Input);
        game.Services.Cache(storage);
        game.Services.Cache(graphics);
        game.Services.Cache(graphics.Factory);
        game.Services.Cache(systems);
        game.Services.Cache(systems.Get<SceneController>());

        var loader = new AssetLoader();
        loader.Register<Effect, EffectLoader>();
        loader.Register<ITexture, TextureLoader>();

        game.Services.Cache(loader);

        threads = new()
        {
            ExecutionMode = options.ExecutionMode,
            FramesPerSecond = options.FramesPerSecond,
            UpdatePerSecond = options.UpdatePerSecond,
        };

        game.Services.Cache(threads);

        foreach (var action in postBuildActions)
            action.Invoke(game);

        return game;
    }

    public void Run()
    {
        Build().Run();
    }

    private static GraphicsAPI getGraphicsAPIForPlatform()
    {
        return RuntimeInfo.OS switch
        {
            RuntimeInfo.Platform.Windows => GraphicsAPI.Direct3D11,
            RuntimeInfo.Platform.Android or RuntimeInfo.Platform.Linux => GraphicsAPI.Vulkan,
            RuntimeInfo.Platform.macOS or RuntimeInfo.Platform.iOS => GraphicsAPI.Metal,
            _ => RuntimeInfo.IsMobile ? GraphicsAPI.OpenGLES : GraphicsAPI.OpenGL,
        };
    }
}
