// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Entities;
using Sekai.Framework.Logging;
using Sekai.Framework.Storage;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public abstract class Host : FrameworkObject
{
    public bool IsRunning => threads != null && threads.IsRunning;

    private Game? game;
    private VirtualStorage? storage;
    private GameSystemRegistry? systems;
    private FrameworkThreadManager? threads;
    protected readonly HostOptions Options;

    protected Host(HostOptions? options = null)
    {
        Options = options ?? new();
    }

    /// <summary>
    /// Starts and runs the game.
    /// </summary>
    public void Run<T>()
        where T : Game, new()
    {
        Logger.OnMessageLogged += new LogListenerConsole();

        if (Game.Current != null)
            throw new InvalidOperationException(@"An active game is currently running. Failed start another instance.");

        Game.Current = game = Activator.CreateInstance<T>();

        game.Services.Cache(this);
        game.Services.Cache(storage = new());
        game.Services.Cache(systems = new(game));
        game.Services.Cache(threads = CreateThreadManager());
        game.Services.Cache(Options);

        systems.Register<SceneManager>();

        threads.Add(new GameUpdateThread(game, systems));
        threads.Post(((ILoadable)game).Load);

        Initialize(game);

        threads.ExecutionMode = Options.ExecutionMode;
        threads.UpdatePerSecond = Options.UpdatePerSecond;
        threads.FramesPerSecond = Options.FramesPerSecond;

        threads.Run();
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Dispose();
    }

    /// <summary>
    /// Creates the threading manager for this host.
    /// </summary>
    protected abstract FrameworkThreadManager CreateThreadManager();

    /// <summary>
    /// Initializes the game before the main loop is started.
    /// </summary>
    protected virtual void Initialize(Game game)
    {
    }

    protected override void Destroy()
    {
        Game.Current = null!;
        game?.Dispose();
        storage?.Dispose();
        systems?.Dispose();
        threads?.Dispose();
    }

    /// <summary>
    /// Gets the appropriate host for the platform it is executing on.
    /// </summary>
    public static Host GetSuitableHost(HostOptions? options = null)
    {
        if (RuntimeInfo.IsDesktop)
            return new DesktopHost(options);

        if (RuntimeInfo.IsMobile)
            return new ViewHost(options);

        throw new PlatformNotSupportedException(@"Failed to find suitable host for the current platform.");
    }

    private class GameUpdateThread : UpdateThread
    {
        private readonly Game game;
        private readonly GameSystemRegistry systems;

        public GameUpdateThread(Game game, GameSystemRegistry systems)
        {
            this.game = game;
            this.systems = systems;
        }

        protected override void OnUpdateFrame(double delta)
        {
            foreach (var system in systems.OfType<IUpdateable>())
                system.Update(delta);

            game.Update(delta);
        }
    }
}
