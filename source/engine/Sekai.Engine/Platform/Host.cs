// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Threading;

namespace Sekai.Engine.Platform;

public abstract class Host : FrameworkObject
{
    private Game? game;
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
        game = Activator.CreateInstance<T>();
        Initialize(game);
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
}
