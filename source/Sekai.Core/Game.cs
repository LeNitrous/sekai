// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Drawing;
using Sekai.Graphics;
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

    private bool isPaused;
    private bool hasLoaded;
    private bool hasCreated;
    private double msPerUpdate = 16.67;
    private double accumulated;
    private double currentTime;
    private double elapsedTime;
    private double previousTime;
    private ISurface? surface;
    private IWaitable? waitable;
    private Stopwatch? stopwatch;
    private GraphicsDevice? graphics;
    private readonly IGameHost host;

    public Game(IGameHost host)
    {
        this.host = host;
        this.host.Create += create;
        this.host.Load += load;
        this.host.Tick += tick;
        this.host.Unload += unload;
        this.host.Paused += paused;
        this.host.Resumed += resumed;
        this.host.Destroy += destroy;
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

    private void create()
    {
        if (hasCreated)
        {
            throw new InvalidOperationException($"Attempting to perform {nameof(IGameHost.Create)} when the game instance has already been created.");
        }

        surface = host.Services.Get<ISurface>(false);
        waitable = host.Services.Get<IWaitableFactory>().CreateWaitable();

        Load();

        hasCreated = true;
    }

    private void load()
    {
        if (!hasCreated)
        {
            throw new InvalidOperationException($"Attempting to perform {nameof(IGameHost.Load)} when the game instance has not yet been initialized.");
        }

        if (hasLoaded)
        {
            throw new InvalidOperationException($"Attempting to perform {nameof(IGameHost.Load)} when the game instance has already been loaded.");
        }

        graphics = surface is not null ? GraphicsDevice.Create(surface, out swapchain) : GraphicsDevice.Create();

        hasLoaded = true;
    }

    private void unload()
    {
        if (!hasLoaded)
        {
            throw new InvalidOperationException($"Attempting to perform {nameof(IGameHost.Unload)} when the game instance has not yet been loaded.");
        }

        if (graphics is not null)
        {
            if (commands is not null)
            {
                graphics.Veldrid.DisposeWhenIdle(commands);
                commands = null;
            }

            if (swapchain is not null && graphics.API != GraphicsAPI.OpenGL)
            {
                graphics.Veldrid.DisposeWhenIdle(swapchain);
                swapchain = null;
            }
            else
            {
                swapchain = null;
            }

            graphics.Veldrid.WaitForIdle();
            graphics.Dispose();
            graphics = null;
        }

        hasLoaded = false;
    }

    private void paused()
    {
        isPaused = true;
    }

    private void resumed()
    {
        isPaused = false;
    }

    private void destroy()
    {
        if (!hasCreated)
        {
            throw new InvalidOperationException($"Attempting to perform {nameof(IGameHost.Destroy)} when the game instance has not been created.");
        }

        host.Create -= create;
        host.Load -= load;
        host.Tick -= tick;
        host.Unload -= unload;
        host.Paused -= paused;
        host.Resumed -= resumed;
        host.Destroy -= destroy;

        Unload();

        waitable!.Dispose();

        hasCreated = false;
        hasLoaded = false;
        isPaused = false;
        accumulated = 0;
        currentTime = 0;
        elapsedTime = 0;
        previousTime = 0;
        stopwatch?.Stop();
        stopwatch = null;
    }

    private void tick()
    {
        if (isPaused || !hasLoaded)
        {
            return;
        }

        if (stopwatch == null)
        {
            stopwatch = new();
            stopwatch.Start();
        }

        bool retry = false;

        do
        {
            if (isPaused || !hasLoaded)
            {
                break;
            }

            currentTime = stopwatch.Elapsed.Milliseconds;
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
                if (hasLoaded && !isPaused)
                {
                    break;
                }

                accumulated -= msPerUpdate;
                stepCount++;
                Update(elapsed);
            }

            // Draw call needs to compensate from multiple update calls.
            elapsed = TimeSpan.FromMilliseconds(msPerUpdate * stepCount);
        }
        else
        {
            elapsed = TimeSpan.FromMilliseconds(elapsedTime);
            Update(elapsed);
        }

        draw(elapsed);
    }

    private Size previousSize;
    private Veldrid.Swapchain? swapchain;
    private Veldrid.CommandList? commands;

    private void draw(TimeSpan elapsed)
    {
        if (graphics is null || swapchain is null)
        {
            return;
        }

        if (surface?.Exists == true && previousSize != surface.Size)
        {
            swapchain.Resize((uint)surface.Size.Width, (uint)surface.Size.Height);
            previousSize = surface.Size;
        }

        try
        {
            commands ??= graphics.Factory.CreateCommandList();
            commands.Begin();
            commands.SetFramebuffer(swapchain.Framebuffer);
            commands.ClearColorTarget(0, Veldrid.RgbaFloat.CornflowerBlue);
            commands.End();

            graphics.Veldrid.SubmitCommands(commands);

            Draw(elapsed);
        }
        finally
        {
            graphics.Veldrid.SwapBuffers(swapchain);
            graphics.Veldrid.WaitForIdle();
        }
    }
}
