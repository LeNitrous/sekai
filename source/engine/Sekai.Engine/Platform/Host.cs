// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Threading;
using Sekai.Framework;
using Sekai.Framework.Containers;
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

        if (view is IWindow window)
        {
            window.Size = options.Size;
            window.Title = options.Title;
            window.Visible = false;
        }

        var systems = new GameSystemCollection();
        systems.Register<SceneManager>();
        systems.Register<BehaviorManager>();

        var updateThread = new MainUpdateThread(systems);
        var renderThread = new MainRenderThread(systems);

        threads = new ThreadController(view);
        threads.Add(updateThread);
        threads.Add(renderThread);
        threads.ExecutionMode = options.ExecutionMode;
        threads.FramesPerSecond = options.FramesPerSecond;
        threads.UpdatePerSecond = options.UpdatePerSecond;

        callbackThreadController?.Invoke(threads);

        Game.AddInternal(systems);
        Game.OnLoad += () =>
        {
            var container = (Game.Container as Container)!;
            container.Cache(this);
            container.Cache(view);
            container.Cache(threads);
            container.Cache(systems);

            callbackGameLoad?.Invoke(Game);

            if (view is IWindow window)
            {
                window.Visible = true;
                container.Cache(window);
            }
        };

        threads.Post(() => Game.Initialize(thread: updateThread));
        threads.Run();
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
    }
}