// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Threading;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Storage;

namespace Sekai.Engine.Platform;

public static class Host
{
    public static HostBuilder<T> Setup<T>(HostOptions? options = null)
        where T : Game, new()
    {
        return new HostBuilder<T>(options);
    }
}

public sealed partial class Host<T> : FrameworkObject
    where T : Game
{
    /// <summary>
    /// Gets whether the host is currently running.
    /// </summary>
    public bool IsRunning => threads?.IsRunning ?? false;

    /// <summary>
    /// Gets the current game.
    /// </summary>
    public readonly T Game;

    private VirtualStorage storage = null!;
    private IGraphicsDevice graphics = null!;
    private ThreadController threads = null!;

    internal Host(T game)
    {
        Game = game;
    }

    /// <summary>
    /// Starts and runs the game.
    /// </summary>
    public void Run()
    {
        if (IsRunning)
            throw new InvalidOperationException(@"This host is already running.");

        storage = Game.Container.Resolve<VirtualStorage>();
        threads = Game.Container.Resolve<ThreadController>();
        graphics = Game.Container.Resolve<IGraphicsDevice>();

        threads.Run(Game.Initialize);
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
