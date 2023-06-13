// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Platform.Dummy;
using Sekai.Storages;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sekai.Platform;

/// <inheritdoc cref="IHost"/>
public class Host : IHost, IHostFactory
{
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

    public event Action? Tick;

    public event Action? Paused;

    public event Action? Resumed;

    public IWindow? Window { get; private set; }

    public Storage? Storage { get; private set; }

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

        if (RuntimeInfo.IsDebug)
        {
            HotReloadCallbackReceiver.OnUpdate += handleAppUpdate;
        }

        ExceptionDispatchInfo? info = null;
        var reset = new ManualResetEventSlim();

        Window = CreateWindow();
        Window.Border = WindowBorder.Resizable;
        Window.Visible = false;
        Window.Resume += Resume;
        Window.Suspend += Pause;
        Window.Closing += Exit;

        Storage = CreateStorage();

        if (RuntimeInfo.IsDebug)
        {
            HotReloadCallbackReceiver.OnUpdate += handleAppUpdate;
        }

        Task.Factory.StartNew(doGameLoop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        while (State != HostState.Exited)
        {
            Window.DoEvents();

            if (info is not null)
            {
                info.Throw();
                info = null;
            }
        }

        reset.Wait();

        Window.Resume -= Resume;
        Window.Suspend -= Pause;
        Window.Closing -= Exit;

        Window.Dispose();
        Window = null;

        if (RuntimeInfo.IsDebug)
        {
            HotReloadCallbackReceiver.OnUpdate -= handleAppUpdate;
        }

        void doGameLoop()
        {
            try
            {
                DoTick();
                Window.Visible = true;

                while (Window.Exists)
                {
                    DoTick();
                }
            }
            catch (Exception e)
            {
                info = ExceptionDispatchInfo.Capture(e);
            }

            reset.Set();
        }
    }

    public void Exit()
    {
        setHostState(HostState.Exiting);
    }

    /// <summary>
    /// Creates a window for this host.
    /// </summary>
    protected virtual IWindow CreateWindow() => new DummyWindow();

    /// <summary>
    /// Creates storage for this host.
    /// </summary>
    protected virtual Storage CreateStorage() => new MountableStorage();

    /// <inheritdoc cref="IHostFactory.CreateGraphics"/>
    protected virtual GraphicsDevice CreateGraphics() => GraphicsDevice.CreateDummy();

    /// <inheritdoc cref="IHostFactory.CreateInput"/>
    protected virtual InputSource CreateInput() => new();

    /// <inheritdoc cref="IHostFactory.CreateAudio"/>
    protected virtual AudioDevice CreateAudio() => AudioDevice.CreateDummy();

    protected void Pause()
    {
        setHostState(HostState.Pausing);
    }

    protected void Resume()
    {
        setHostState(HostState.Resuming);
    }

    protected void Restart()
    {
        setHostState(HostState.Restarting);
    }

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

    private void handleAppUpdate(Type[]? types)
    {
        lock (sync)
        {
            while (!isValidTransition(state, HostState.Restarting)) ;
        }

        Restart();
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

    InputSource IHostFactory.CreateInput() => CreateInput();
    AudioDevice IHostFactory.CreateAudio() => CreateAudio();
    GraphicsDevice IHostFactory.CreateGraphics() => CreateGraphics();

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
