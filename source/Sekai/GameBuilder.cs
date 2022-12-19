// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sekai.Allocation;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Scenes;
using Sekai.Storage;
using Sekai.Windowing;

namespace Sekai;

public sealed class GameBuilder<T>
    where T : Game, new()
{
    private readonly T game;
    private readonly GameOptions options;
    private GameRunner? runner;
    private Lazy<IView>? view;
    private Lazy<IAudioSystem>? audio;
    private Lazy<IGraphicsSystem>? graphics;
    private readonly Queue<Action> preBuildAction = new();
    private readonly Queue<Action> postBuildAction = new();

    internal GameBuilder(T game, GameOptions? options = null)
    {
        this.game = game;
        this.options = options ?? new();
    }

    public GameBuilder<T> UseAudio<U>()
        where U : IAudioSystem, new()
    {
        audio = new(() => new U());
        return this;
    }

    /// <summary>
    /// Uses the view of a given type.
    /// </summary>
    public GameBuilder<T> UseView<U>()
        where U : IView, new()
    {
        view = new(() => new U());
        return this;
    }

    /// <summary>
    /// Uses the graphics system of a given type.
    /// </summary>
    public GameBuilder<T> UseGraphics<U>()
        where U : IGraphicsSystem, new()
    {
        graphics = new(() => new U());
        return this;
    }

    /// <summary>
    /// Adds an action invoked as the game is loaded.
    /// </summary>
    public GameBuilder<T> AddLoadAction(Action action)
    {
        game.OnLoaded += action;
        return this;
    }

    /// <summary>
    /// Adds an action invoked as the game exits.
    /// </summary>
    public GameBuilder<T> AddExitAction(Action action)
    {
        game.OnExiting += action;
        return this;
    }

    /// <summary>
    /// Adds an action invoked before <see cref="Build"/> is invoked.
    /// </summary>
    public GameBuilder<T> AddPreBuildAction(Action action)
    {
        preBuildAction.Enqueue(action);
        return this;
    }

    /// <summary>
    /// Adds an action invoked after <see cref="Build"/> is invoked.
    /// </summary>
    public GameBuilder<T> AddPostBuildAction(Action action)
    {
        postBuildAction.Enqueue(action);
        return this;
    }

    /// <summary>
    /// Builds the game.
    /// </summary>
    public T Build()
    {
        Services.Initialize();

        if (RuntimeInfo.IsDebug)
            Logger.OnMessageLogged += new LogListenerConsole();

        while (preBuildAction.TryDequeue(out var preBuildMethod))
            preBuildMethod.Invoke();

        var storage = new StorageContext();
        storage.Mount("/", new NativeStorage(AppDomain.CurrentDomain.BaseDirectory));
        storage.Mount("/engine", new AssemblyBackedStorage(typeof(Game).Assembly, "Resources"));

        Services.Current.Cache(storage);

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

        if (graphics is null || view is null)
            throw new InvalidOperationException(@"Cannot build the game without a graphics system or view.");

        Services.Current.Cache(game);
        Services.Current.Cache<Game>(game);
        Services.Current.Cache(options);
        Services.Current.Cache(new GraphicsContext(graphics.Value, view.Value));
        Services.Current.Cache(runner = new());

        if (view.Value is IWindow window)
        {
            window.Size = options.Size;
            window.Title = options.Title;
            window.Border = WindowBorder.Resizable;
            window.OnClose += game.Exit;
            runner.OnTick += showWindow;
        }

        if (audio is not null)
            Services.Current.Cache(new AudioContext());

        Services.Current.Cache<InputContext>();
        Services.Current.Cache<SceneCollection>();

        while (postBuildAction.TryDequeue(out var postBuildMethod))
            postBuildMethod.Invoke();

        return game;
    }

    /// <summary>
    /// Builds then runs the game.
    /// </summary>
    public void Run() => Build().Run();

    private void showWindow()
    {
        if (view is not null && view.Value is IWindow window)
            window.Visible = true;

        if (runner is not null)
            runner.OnTick -= showWindow;
    }
}
