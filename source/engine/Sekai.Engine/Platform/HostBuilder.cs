// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using Sekai.Engine.Effects.Compiler;
using Sekai.Engine.Resources;
using Sekai.Engine.Threading;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Storage;
using Sekai.Framework.Threading;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Platform;

public sealed class HostBuilder<T>
    where T : Game, new()
{
    private readonly T game;
    private readonly Host<T> host;
    private readonly HostOptions options;
    private IWindow window = null!;
    private IGraphicsDevice graphics = null!;
    private Action<T> callbackGameLoad = null!;
    private ThreadController threads = null!;
    private readonly VirtualStorage storage = new();
    private readonly List<Action> postSetupActions = new();
    private readonly SystemCollection<GameSystem> systems = new();

    internal HostBuilder(HostOptions? options = null)
    {
        game = new T();
        host = new Host<T>(game);
        this.options = options ?? new();
    }

    /// <summary>
    /// Uses the window of a given type.
    /// </summary>
    public HostBuilder<T> UseWindow<U>()
        where U : IWindow, new()
    {
        window = new U();
        return this;
    }

    /// <summary>
    /// Uses the graphics context of a given type.
    /// </summary>
    public HostBuilder<T> UseGraphics<U>()
        where U : IGraphicsDevice, new()
    {
        graphics = new U();
        return this;
    }

    /// <summary>
    /// Uses a callback that is invoked after the game has finished loading.
    /// </summary>
    public HostBuilder<T> UseLoadCallback(Action<T> callback)
    {
        callbackGameLoad = callback;
        return this;
    }

    /// <summary>
    /// Registers a game system.
    /// </summary>
    public HostBuilder<T> Register<U>()
        where U : GameSystem, new()
    {
        postSetupActions.Add(() => systems.Register<U>());
        return this;
    }

    /// <summary>
    /// Mounts storage to the virtual file system.
    /// </summary>
    public HostBuilder<T> Mount(string basePath, IStorage storage)
    {
        postSetupActions.Add(() => this.storage.Mount(basePath, storage));
        return this;
    }

    /// <summary>
    /// Caches an instance to the root container.
    /// </summary>
    public HostBuilder<T> Cache<U>(U instance)
    {
        postSetupActions.Add(() => game.Container.Cache(instance));
        return this;
    }

    /// <summary>
    /// Adds a thread to the thread controller.
    /// </summary>
    public HostBuilder<T> AddThread(FrameworkThread thread)
    {
        postSetupActions.Add(() => threads.Add(thread));
        return this;
    }

    /// <summary>
    /// Creates a host using configurations provided.
    /// </summary>
    public Host<T> Build()
    {
        if (RuntimeInfo.IsDebug)
            Logger.OnMessageLogged += new LogListenerConsole();

        systems.Register<SceneController>();

        window.Size = options.Size;
        window.Title = options.Title;
        window.Border = WindowBorder.Resizable;
        window.OnClose += host.Exit;

        options.Graphics.Debug ??= RuntimeInfo.IsDebug;
        options.Graphics.GraphicsAPI ??= getGraphicsAPIForPlatform();

        graphics.Initialize(window, options.Graphics);
        Logger.Log($"{graphics.GraphicsAPI} Initialized");
        Logger.Log($"{graphics.GraphicsAPI} Device: {graphics.Name}");

        storage.Mount("/engine", new AssemblyBackedStorage(ResourceAssembly.Assembly));
        storage.Mount("/engine/logs", new NativeStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")));
        storage.Mount("/engine/cache", new NativeStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache")));

        var updateThread = new MainUpdateThread(systems);
        var renderThread = new MainRenderThread(systems, graphics);

        threads = new ThreadController(window);
        threads.Add(updateThread);
        threads.Add(renderThread);

        threads.ExecutionMode = options.ExecutionMode;
        threads.FramesPerSecond = options.FramesPerSecond;
        threads.UpdatePerSecond = options.UpdatePerSecond;

        game.Container.Cache(host);
        game.Container.Cache(threads);
        game.Container.Cache(systems);
        game.Container.Cache(storage);
        game.Container.Cache(window.Input);
        game.Container.Cache(graphics);
        game.Container.Cache(graphics.Factory);
        game.Container.Cache(new EffectCompiler(graphics));
        game.Container.Cache<IView>(window);

        game.AddInternal(systems);

        game.OnLoad += () =>
        {
            callbackGameLoad?.Invoke(game);
            window.Visible = true;
        };

        foreach (var action in postSetupActions)
            action.Invoke();

        return host;
    }

    /// <summary>
    /// Builds then runs the host.
    /// </summary>
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
