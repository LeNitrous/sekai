// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Entities;
using Sekai.Framework.Graphics;
using Sekai.Framework.Storage;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public abstract class Host : FrameworkObject
{
    /// <summary>
    /// Gets whether the host is currently running or not.
    /// </summary>
    public bool IsRunning => threads != null && threads.IsRunning;

    private Game? game;
    private VirtualStorage? storage;
    private IGraphicsContext? graphics;
    private GameSystemRegistry? systems;
    private FrameworkThreadManager? threads;
    protected readonly HostOptions Options;
    protected RenderThread? RenderThread { get; private set; }
    protected UpdateThread? UpdateThread { get; private set; }

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
        if (Game.Current != null)
            throw new InvalidOperationException(@"An active game is currently running. Failed start another instance.");

        Game.Current = game = Activator.CreateInstance<T>();

        game.Services.Cache(this);
        game.Services.Cache(storage = new());
        game.Services.Cache(systems = new(game));
        game.Services.Cache(threads = CreateThreadManager());
        game.Services.Cache(graphics = CreateGraphicsContext(Options.Renderer));
        game.Services.Cache(Options);

        systems.Register<SceneManager>();

        threads.Add(UpdateThread = new GameUpdateThread(game, systems));
        threads.Add(RenderThread = new GameRenderThread(game, systems));
        threads.Post(game.LoadInternal);

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
    /// Creates the graphics context for this host.
    /// </summary>
    protected abstract IGraphicsContext CreateGraphicsContext(GraphicsAPI api);

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
        graphics?.Dispose();
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
            if (!game.IsLoaded)
                return;

            systems.Update(delta);
            game.Update(delta);
        }
    }

    private class GameRenderThread : RenderThread
    {
        private readonly Game game;
        private readonly GameSystemRegistry systems;

        public GameRenderThread(Game game, GameSystemRegistry systems)
        {
            this.game = game;
            this.systems = systems;
        }

        protected override void OnRenderFrame(CommandList commands)
        {
            if (!game.IsLoaded)
                return;

            systems.Render(commands);
            game.Render(commands);
        }
    }
}
