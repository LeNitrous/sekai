// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Threading;
using Sekai.Framework;
using Sekai.Framework.Storage;

namespace Sekai.Engine;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : FrameworkObject
{
    /// <summary>
    /// The current game instance.
    /// </summary>
    public static Game Current
    {
        get
        {
            if (current is null)
                throw new InvalidOperationException(@"There is no active game instance present.");

            return current;
        }
        set
        {
            if (current is not null)
                throw new InvalidOperationException(@"There is an active game instance currently running.");

            current = value;
        }
    }

    private static Game? current = null;

    /// <summary>
    /// Prepares a game for running.
    /// </summary>
    public static GameBuilder<T> Setup<T>(GameOptions? options = null)
        where T : Game, new()
    {
        return new GameBuilder<T>((T)(Current = new T()), options);
    }

    /// <summary>
    /// The services associated with this game.
    /// </summary>
    public Container Services { get; } = new Container();

    /// <summary>
    /// Called when the game is loaded.
    /// </summary>
    public event Action OnLoad = null!;

    /// <summary>
    /// Called as the game is being unloaded.
    /// </summary>
    public event Action OnUnload = null!;

    public Game()
    {
        OnLoad += Load;
        OnUnload += Unload;
    }

    /// <summary>
    /// Called as the game starts.
    /// </summary>
    public virtual void Load()
    {
    }

    /// <summary>
    /// Called before the game exits.
    /// </summary>
    public virtual void Unload()
    {
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Run()
    {
        Services.Resolve<ThreadController>().Run();
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Services.Resolve<ThreadController>().Dispose();
    }

    protected sealed override void Destroy()
    {
        if (Services.Resolve<ThreadController>().IsRunning)
            throw new InvalidOperationException($"Cannot dispose the game instance. Use {nameof(Game.Exit)} to close the game.");

        Unload();
        Services.Resolve<VirtualStorage>().Dispose();
    }
}
