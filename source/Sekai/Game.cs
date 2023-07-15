// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Logging;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// The entry point for Sekai.
/// </summary>
public abstract class Game
{
    /// <summary>
    /// The host running this game.
    /// </summary>
    public Host Host { get; internal set; } = null!;

    /// <summary>
    /// The window backing this game.
    /// </summary>
    public IWindow Window { get; internal set; } = null!;

    /// <summary>
    /// The storage backing this game.
    /// </summary>
    public Storage Storage { get; internal set; } = null!;

    /// <summary>
    /// The input state of the host.
    /// </summary>
    public InputState Input { get; internal set; } = null!;

    /// <summary>
    /// The audio device providing audio for this game.
    /// </summary>
    public AudioDevice Audio { get; internal set; } = null!;

    /// <summary>
    /// The graphics device providing graphics for this game.
    /// </summary>
    public GraphicsDevice Graphics { get; internal set; } = null!;

    /// <summary>
    /// The logger used for logging.
    /// </summary>
    public Logger Logger { get; internal set; } = null!;

    /// <summary>
    /// The options passed during initialization.
    /// </summary>
    public HostOptions Options { get; internal set; } = null!;

    /// <summary>
    /// Called once at the beginning.
    /// </summary>
    public virtual void Load()
    {
    }

    /// <summary>
    /// Called once every frame.
    /// </summary>
    public virtual void Draw()
    {
    }

    /// <summary>
    /// Called once every frame when <see cref="TickMode.Variable"/> or possibly multiple times when <see cref="TickMode.Fixed"/>.
    /// </summary>
    /// <param name="elapsed">The time between frames.</param>
    public virtual void Update(TimeSpan elapsed)
    {
    }

    /// <summary>
    /// Called once before closing the game.
    /// </summary>
    public virtual void Unload()
    {
    }
}
