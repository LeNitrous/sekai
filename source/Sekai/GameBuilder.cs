// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
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

    private IView? view;
    private IAudioSystem? audio;
    private IGraphicsSystem? graphics;
    private readonly T game;
    private readonly GameOptions options;
    private GameRunner? runner;

    internal GameBuilder(T game, GameOptions? options = null)
    {
        this.game = game;
        this.options = options ?? new();
    }

    public GameBuilder<T> UseAudio<U>()
        where U : IAudioSystem, new()
    {
        audio = new U();
        return this;
    }

    /// <summary>
    /// Uses the view of a given type.
    /// </summary>
    public GameBuilder<T> UseView<U>()
        where U : IView, new()
    {
        view = new U();
        return this;
    }

    /// <summary>
    /// Uses the graphics system of a given type.
    /// </summary>
    public GameBuilder<T> UseGraphics<U>()
        where U : IGraphicsSystem, new()
    {
        graphics = new U();
        return this;
    }

    /// <summary>
    /// Builds the game.
    /// </summary>
    public T Build()
    {
        if (RuntimeInfo.IsDebug)
            Logger.OnMessageLogged += new LogListenerConsole();

        var storage = new StorageContext();
        storage.Mount("/", new NativeStorage(AppDomain.CurrentDomain.BaseDirectory));
        storage.Mount("/engine", new AssemblyBackedStorage(typeof(Game).Assembly, "Resources"));

        Services.Current.Register(storage);

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

        Services.Current.Register(game);
        Services.Current.Register<Game>(game);
        Services.Current.Register(options);
        Services.Current.Register(new GraphicsContext(graphics, view));
        Services.Current.Register(runner = new());

        if (view is IWindow window)
        {
            window.Size = options.Size;
            window.Title = options.Title;
            window.Border = WindowBorder.Resizable;
            window.OnClose += game.Exit;
            runner.OnTick += showWindow;
        }

        if (audio is not null)
            Services.Current.Register(new AudioContext());

        Services.Current.Register<InputContext>();
        Services.Current.Register<SceneCollection>();

        return game;
    }

    /// <summary>
    /// Builds then runs the game.
    /// </summary>
    public void Run() => Build().Run();

    private void showWindow()
    {
        if (view is IWindow window)
            window.Visible = true;

        if (runner is not null)
            runner.OnTick -= showWindow;
    }
}
