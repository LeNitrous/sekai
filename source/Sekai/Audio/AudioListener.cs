// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Audio;

/// <summary>
/// An audio listener.
/// </summary>
public abstract class AudioListener : IDisposable
{
    /// <summary>
    /// Gets or sets the volume of the listener.
    /// </summary>
    public abstract float Volume { get; set; }

    /// <summary>
    /// Gets or sets the position of the listener.
    /// </summary>
    public abstract Vector3 Position { get; set; }

    /// <summary>
    /// Gets or sets the velocity of the listener.
    /// </summary>
    public abstract Vector3 Velocity { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the listener.
    /// </summary>
    public abstract ListenerOrientation Orientation { get; set; }

    public abstract void Dispose();
}
