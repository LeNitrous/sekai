// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Effects.Compiler;
using Sekai.Engine.Threading;
using Sekai.Framework;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Platform;

public static class Host
{
    public static Host<T> Setup<T>(HostOptions? options = null)
        where T : Game, new()
    {
        return new Host<T>(new T(), options);
    }
}

public sealed partial class Host<T> : FrameworkObject
    where T : Game
{
    public bool IsRunning => threads?.IsRunning ?? false;
    public T Game = null!;
    private readonly HostOptions options;
    private ThreadController threads = null!;

    internal Host(T game, HostOptions? options = null)
    {
        Game = game;
        this.options = options ?? new();
    }

    /// <summary>
    /// Starts and runs the game.
    /// </summary>
    public void Run()
    {
        if (IsRunning)
            throw new InvalidOperationException(@"This host is already running.");

        setupHostInstances();

        var systems = new SystemCollection<GameSystem>();
        systems.Register<SceneController>();

        var updateThread = new MainUpdateThread(systems);
        var renderThread = new MainRenderThread(systems, graphics);

        threads = new ThreadController(window);
        threads.Add(updateThread);
        threads.Add(renderThread);
        threads.ExecutionMode = options.ExecutionMode;
        threads.FramesPerSecond = options.FramesPerSecond;
        threads.UpdatePerSecond = options.UpdatePerSecond;

        Game.Container.Cache(this);
        Game.Container.Cache(threads);
        Game.Container.Cache(systems);
        Game.Container.Cache(storage);
        Game.Container.Cache(window.Input);
        Game.Container.Cache(graphics);
        Game.Container.Cache(graphics.Factory);
        Game.Container.Cache(new EffectCompiler(graphics));
        Game.Container.Cache<IView>(window);

        Game.AddInternal(systems);

        Game.OnLoad += () =>
        {
            callbackGameLoad?.Invoke(Game);
            window.Visible = true;
        };

        threads.Run(() => updateThread.Post(Game.Initialize));
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Dispose();
    }

    protected override void Destroy()
    {
        if (!IsRunning)
            return;

        Game.Dispose();
        threads.Dispose();
        storage.Dispose();
        graphics.Dispose();
    }
}
