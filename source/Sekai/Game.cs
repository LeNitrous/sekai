// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Drawing;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Platform;

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
        get => 1000 / msPerUpdate.Milliseconds;
        set
        {
            if (value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than zero.");
            }

            msPerUpdate = TimeSpan.FromSeconds(1 / value);
        }
    }

    /// <summary>
    /// The game's input manager.
    /// </summary>
    protected InputManager? Input { get; private set; }

    /// <summary>
    /// The game's audio device.
    /// </summary>
    protected AudioDevice? Audio { get; private set; }

    /// <summary>
    /// The game's graphics device.
    /// </summary>
    protected GraphicsDevice? Graphics { get; private set; }

    private Host? host;
    private Waiter waitable;
    private TimeSpan msPerUpdate = TimeSpan.FromSeconds(1 / 120.0);
    private TimeSpan accumulated;
    private TimeSpan currentTime;
    private TimeSpan elapsedTime;
    private TimeSpan previousTime;
    private readonly Stopwatch stopwatch = new();
    private static readonly TimeSpan wait_threshold = TimeSpan.FromMilliseconds(2);

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

        waitable = new Waiter();

        Input = new InputManager(((IHostFactory)host).CreateInput());
        Audio = ((IHostFactory)host).CreateAudio();
        Graphics = ((IHostFactory)host).CreateGraphics();

        stopwatch.Start();

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

        accumulated = TimeSpan.Zero;
        currentTime = TimeSpan.Zero;
        elapsedTime = TimeSpan.Zero;
        previousTime = TimeSpan.Zero;

        host.Tick -= tick;
        host.Paused -= pause;
        host.Resumed -= resume;

        waitable.Dispose();

        Input = null;

        Audio?.Dispose();
        Audio = null;

        Graphics?.Dispose();
        Graphics = null;

        stopwatch.Reset();

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
        stopwatch.Stop();
    }

    private void resume()
    {
        stopwatch.Start();
    }

    private void tick()
    {
        if (!stopwatch.IsRunning)
        {
            return;
        }

        bool retry = false;

        do
        {
            if (!stopwatch.IsRunning)
            {
                break;
            }

            currentTime = stopwatch.Elapsed;
            elapsedTime = currentTime - previousTime;
            accumulated += elapsedTime;
            previousTime = currentTime;

            if (TickMode == TickMode.Fixed && accumulated < msPerUpdate)
            {
                var duration = msPerUpdate - accumulated;

                // Timers are at the mercy of the operating system's scheduler. Thus sleep() can be
                // very innacurate. It can end earlier or later than the requested time. It depends
                // when the scheduler gives back control to our application.

                // To save on resources, we avoid using a spin lock for the entire duration of the
                // wait. We can use sleep() first to give back control to the scheduler and free up
                // CPU usage for the duration less the threshold we allow it to.
                if (duration >= wait_threshold)
                {
                    waitable.Wait(duration - wait_threshold);
                }

                // After the wait, we still have a bit more time remaining. To accurately finish off,
                // we use a spin lock. Because of the earlier wait, we have less iterations to do in
                // this spin lock.
                while (duration > stopwatch.Elapsed - currentTime) ;

                retry = true;
            }
            else
            {
                retry = false;
            }
        }
        while (retry);

        if (TickMode == TickMode.Fixed)
        {
            while (accumulated >= msPerUpdate)
            {
                if (!stopwatch.IsRunning)
                {
                    break;
                }

                accumulated -= msPerUpdate;
                update(msPerUpdate);
            }
        }
        else
        {
            update(elapsedTime);
        }

        draw();
    }

    private void draw()
    {
        if (host?.Window is null || Graphics is null)
        {
            return;
        }

        Graphics.SetViewport(new Rectangle(Point.Empty, host.Window.Size));
        Graphics.SetScissor(new Rectangle(Point.Empty, host.Window.Size));
        Graphics.Clear(new ClearInfo(Color.CornflowerBlue));

        Draw();

        Graphics.Present();
    }

    private void update(TimeSpan elapsed)
    {
        Input?.Update();
        Update(elapsed);
    }
}
