// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Platform;

/// <summary>
/// Hosts a <see cref="Game"/> and performs its lifecycle events.
/// </summary>
public abstract class Host
{
    /// <summary>
    /// The host's state.
    /// </summary>
    public HostState State
    {
        get
        {
            lock (sync)
            {
                return state;
            }
        }
    }

    /// <summary>
    /// The host's view.
    /// </summary>
    public abstract IView? View { get; }

    /// <summary>
    /// Called when the host ticks.
    /// </summary>
    public event Action? Tick;

    /// <summary>
    /// Called when the host has paused.
    /// </summary>
    public event Action? Paused;

    /// <summary>
    /// Called when the host has resumed from pausing.
    /// </summary>
    public event Action? Resumed;

    private Game? game;
    private HostState state;
    private readonly object sync = new();

    /// <summary>
    /// Runs the game.
    /// </summary>
    /// <param name="game">The game to run.</param>
    public void Run(Game game)
    {
        if (State < HostState.Idle)
        {
            return;
        }

        this.game = game;

        setHostState(HostState.Loading);

        Run();
    }

    /// <summary>
    /// Runs the host.
    /// </summary>
    protected abstract void Run();

    /// <summary>
    /// Requests the host to exit. The request is honored on the next <see cref="DoTick"/> call.
    /// </summary>
    protected void Exit()
    {
        setHostState(HostState.Exiting);
    }

    /// <summary>
    /// Requests the host to pause. The request is honored on the next <see cref="DoTick"/> call.
    /// </summary>
    protected void Pause()
    {
        setHostState(HostState.Pausing);
    }

    /// <summary>
    /// Requests the host to resume from pause. The request is honored on the next <see cref="DoTick"/> call.
    /// </summary>
    protected void Resume()
    {
        setHostState(HostState.Resuming);
    }

    /// <summary>
    /// Requests the host to restart. The request is honored on the next <see cref="DoTick"/> call.
    /// </summary>
    protected void Restart()
    {
        setHostState(HostState.Restarting);
    }

    /// <summary>
    /// Updates its state and performs events based on its updated state.
    /// </summary>
    protected void DoTick()
    {
        switch (State)
        {
            case HostState.Loading:
                game?.Attach(this);
                setHostState(HostState.Running);
                break;

            case HostState.Running:
                Tick?.Invoke();
                break;

            case HostState.Pausing:
                Paused?.Invoke();
                setHostState(HostState.Paused);
                break;

            case HostState.Resumed:
                Resumed?.Invoke();
                setHostState(HostState.Running);
                break;

            case HostState.Restarting:
                game?.Detach(this);
                setHostState(HostState.Loading);
                break;

            case HostState.Resuming:
                setHostState(HostState.Resumed);
                break;

            case HostState.Exiting:
                game?.Detach(this);
                setHostState(HostState.Exited);
                break;
        }
    }

    private void setHostState(HostState next)
    {
        lock (sync)
        {
            if (!isValidTransition(state, next))
            {
                throw new InvalidOperationException($"Invalid state transition {state} -> {next}.");
            }

            state = next;
        }
    }

    private static bool isValidTransition(HostState current, HostState next)
    {
        switch (next)
        {
            case HostState.Loading:
                return current is HostState.Idle or HostState.Restarting;

            case HostState.Running:
                return current is HostState.Loading or HostState.Resuming;

            case HostState.Paused:
                return current is HostState.Pausing;

            case HostState.Resuming:
                return current is HostState.Paused;

            case HostState.Resumed:
                return current is HostState.Resuming;

            case HostState.Exited:
                return current is HostState.Exiting;

            case HostState.Exiting:
            case HostState.Restarting:
                return current is HostState.Running or HostState.Pausing or HostState.Paused or HostState.Resuming or HostState.Resumed;

            default:
                return false;
        }
    }
}
