// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Audio;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Storages;
using Sekai.Framework.Windowing;

namespace Sekai;

/// <summary>
/// The entry point for Sekai.
/// </summary>
public class Game
{
    /// <summary>
    /// The host running this game.
    /// </summary>
    public Host Host
    {
        get => host ?? throw new InvalidOperationException("The game has not yet been loaded.");
        internal set => host = value;
    }

    /// <summary>
    /// The window backing this game.
    /// </summary>
    public IWindow Window => Host.Window;

    /// <summary>
    /// The input state of the host.
    /// </summary>
    public InputState Input => Host.Input;

    /// <summary>
    /// The audio device providing audio for this game.
    /// </summary>
    public AudioDevice Audio => Host.Audio;

    /// <summary>
    /// The graphics device providing graphics for this game.
    /// </summary>
    public GraphicsDevice Graphics => Host.Graphics;

    /// <summary>
    /// The logger used for logging.
    /// </summary>
    public Logger Logger => Host.Logger;

    /// <summary>
    /// The storage provided for this game.
    /// </summary>
    public MountableStorage Storage => Host.Storage;

    private Host? host;

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
