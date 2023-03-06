// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sekai.Platform;
using Sekai.Platform.Windows;

namespace Sekai;

/// <summary>
/// The game class implementing the lifecycle.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets or sets the ticking logic performed by <see cref="Tick"/>.
    /// </summary>
    /// <remarks>
    /// When set to <see cref="TickMode.Fixed"/>, the target time between frames is provided by <see cref="UpdatePerSecond"/>.
    /// </remarks>
    public TickMode TickMode { get; set; } = TickMode.Fixed;

    /// <summary>
    /// Gets or sets the update rate of the game when <see cref="IsFixedTimeStep"/> is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// The returned value may be an approximation of the originally set value.
    /// </remarks>
    public double UpdatePerSecond
    {
        get => 1000 / msPerUpdate;
        set
        {
            if (value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than zero.");
            }

            msPerUpdate = 1 / value * 1000;
        }
    }

    /// <summary>
    /// Gets the game's services.
    /// </summary>
    public ServiceLocator Services { get; }

    private bool isRunning;
    private bool isExiting;
    private double msPerUpdate = 16.67;
    private double accumulated;
    private double currentTime;
    private double elapsedTime;
    private double previousTime;
    private Stopwatch? stopwatch;
    private readonly IWaitable waitable;

    public Game()
    {
        Services = new();
        waitable = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new WindowsWaitableObject() : new DefaultWaitableObject();
    }

    protected virtual void Load()
    {
    }

    protected virtual void Draw(TimeSpan elapsed)
    {
    }

    protected virtual void Update(TimeSpan elapsed)
    {
    }

    protected virtual void Unload()
    {
    }

    /// <summary>
    /// Runs the game on the calling thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the game is already running.</exception>
    public void Run()
    {
        if (isRunning)
        {
            throw new InvalidOperationException("The game is already running.");
        }

        run();
    }

    /// <summary>
    /// Runs the game asynchronously.
    /// </summary>
    /// <returns>The task representing the run action.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the game is already running.</exception>
    public Task RunAsync()
    {
        if (isRunning)
        {
            throw new InvalidOperationException("The game is already running.");
        }

        return Task.Factory.StartNew(run, default, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        isExiting = true;
    }

    /// <summary>
    /// Resets the game time back to zero.
    /// </summary>
    public void Reset()
    {
        accumulated = 0;
        currentTime = 0;
        elapsedTime = 0;
        previousTime = 0;
        stopwatch?.Reset();
    }

    /// <summary>
    /// Performs an iteration of the game loop.
    /// </summary>
    public void Tick()
    {
        if (isRunning)
        {
            throw new InvalidOperationException($"Cannot externally call {nameof(Tick)} when the game is currently running.");
        }

        tick();
    }

    private void run()
    {
        Reset();

        load();

        isRunning = true;
        isExiting = false;

        while (!isExiting)
        {
            tick();
        }

        unload();

        isRunning = false;
    }

    private void load()
    {
        Load();
    }

    private void unload()
    {
        Unload();
    }

    private void tick()
    {
        if (stopwatch == null)
        {
            stopwatch = new();
            stopwatch.Start();
        }

        bool retry = false;

        do
        {
            if (isExiting)
            {
                break;
            }

            currentTime = stopwatch.Elapsed.Milliseconds;
            elapsedTime = currentTime - previousTime;
            accumulated += elapsedTime;
            previousTime = currentTime;

            if (TickMode == TickMode.Fixed && accumulated < msPerUpdate)
            {
                waitable.Wait(TimeSpan.FromMilliseconds(msPerUpdate - accumulated));
                retry = true;
            }
            else
            {
                retry = false;
            }
        }
        while (retry);

        TimeSpan elapsed;

        if (TickMode == TickMode.Fixed)
        {
            int stepCount = 0;

            elapsed = TimeSpan.FromMilliseconds(msPerUpdate);

            while (accumulated >= msPerUpdate && !isExiting)
            {
                update(elapsed);
                accumulated -= msPerUpdate;
                stepCount++;
            }

            // doDraw needs to compensate from multiple update calls.
            elapsed = TimeSpan.FromMilliseconds(msPerUpdate * stepCount);
        }
        else
        {
            elapsed = TimeSpan.FromMilliseconds(elapsedTime);
            update(elapsed);
        }

        draw(elapsed);
    }

    private void draw(TimeSpan elapsed)
    {
        Draw(elapsed);
    }

    private void update(TimeSpan elapsed)
    {
        Update(elapsed);
    }
}
