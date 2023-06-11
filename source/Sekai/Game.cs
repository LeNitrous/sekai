// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Drawing;
using Sekai.Graphics;
using Sekai.Platform;
using Sekai.Platform.Windows;

namespace Sekai;

/// <summary>
/// The game class.
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

    protected GraphicsDevice? Graphics;

    private bool isPaused;
    private double msPerUpdate = 1 / 120.0;
    private double accumulated;
    private double currentTime;
    private double elapsedTime;
    private double previousTime;
    private Host? host;
    private IWaitable? waitable;
    private Stopwatch? stopwatch;

    /// <summary>
    /// Attaches the host to this game.
    /// </summary>
    /// <param name="host">The host to attach.</param>
    internal void Attach(Host host)
    {
        if (this.host is not null)
        {
            return;
        }

        host.Tick += tick;
        host.Paused += pause;
        host.Resumed += resume;

        waitable = RuntimeInfo.IsWindows ? new WindowsWaitableObject() : new WaitableObject();
        Graphics = host.View is not null ? GraphicsDevice.Create(host.View.Surface) : GraphicsDevice.CreateDummy();

        this.host = host;

        Load();
    }

    /// <summary>
    /// Detaches the host from this game.
    /// </summary>
    /// <param name="host">The host to detach.</param>
    internal void Detach(Host host)
    {
        if (this.host is not null && !ReferenceEquals(this.host, host))
        {
            return;
        }

        Unload();

        isPaused = false;
        accumulated = 0;
        currentTime = 0;
        elapsedTime = 0;
        previousTime = 0;

        stopwatch?.Stop();
        stopwatch = null;

        host.Tick -= tick;
        host.Paused -= pause;
        host.Resumed -= resume;

        Graphics?.Dispose();
        waitable?.Dispose();

        this.host = null;
    }

    /// <summary>
    /// Called before the first tick for the game to perform initialization.
    /// </summary>
    protected virtual void Load()
    {
    }

    /// <summary>
    /// Called once every frame when <see cref="TickMode.Variable"/> or possibly multiple times when <see cref="TickMode.Fixed"/>.
    /// </summary>
    /// <param name="elapsed">The delta time between frames.</param>
    protected virtual void Update(TimeSpan elapsed)
    {
    }

    /// <summary>
    /// Called once every frame.
    /// </summary>
    protected virtual void Draw()
    {
    }

    /// <summary>
    /// Called before the game exits.
    /// </summary>
    protected virtual void Unload()
    {
    }

    private void pause()
    {
        isPaused = true;
    }

    private void resume()
    {
        isPaused = false;
    }

    private void tick()
    {
        if (stopwatch == null)
        {
            stopwatch = new();
            stopwatch.Start();
        }

        if (stopwatch.IsRunning && isPaused)
        {
            stopwatch.Stop();
            return;
        }
        else
        {
            stopwatch.Start();
        }

        bool retry = false;

        do
        {
            if (isPaused)
            {
                break;
            }

            currentTime = stopwatch.ElapsedMilliseconds;
            elapsedTime = currentTime - previousTime;
            accumulated += elapsedTime;
            previousTime = currentTime;

            if (TickMode == TickMode.Fixed && accumulated < msPerUpdate)
            {
                waitable?.Wait(TimeSpan.FromMilliseconds(msPerUpdate - accumulated));
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

            while (accumulated >= msPerUpdate)
            {
                if (isPaused)
                {
                    break;
                }

                accumulated -= msPerUpdate;
                stepCount++;
                update(elapsed);
            }

            // Draw call needs to compensate from multiple update calls.
            elapsed = TimeSpan.FromMilliseconds(msPerUpdate * stepCount);
        }
        else
        {
            elapsed = TimeSpan.FromMilliseconds(elapsedTime);
            update(elapsed);
        }

        draw();
    }

    private void draw()
    {
        if (host?.View is null)
        {
            return;
        }

        if (Graphics is null)
        {
            return;
        }

        Graphics.SetViewport(new Rectangle(Point.Empty, host.View.Size));
        Graphics.SetScissor(new Rectangle(Point.Empty, host.View.Size));
        Graphics.Clear(new ClearInfo(Color.CornflowerBlue));

        Draw();

        Graphics.Present();
    }

    private void update(TimeSpan elapsed)
    {
        Update(elapsed);
    }
}
