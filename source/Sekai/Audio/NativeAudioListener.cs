// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Audio;

public abstract class NativeAudioListener : DisposableObject
{
    /// <summary>
    /// Gets or sets the listener volume.
    /// </summary>
    public abstract float Volume { get; set; }

    /// <summary>
    /// Gets or sets the listener position.
    /// </summary>
    public abstract Vector3 Position { get; set; }

    /// <summary>
    /// Gets or sets the listener velocity.
    /// </summary>
    public abstract Vector3 Velocity { get; set; }

    /// <summary>
    /// Gets or sets the listener orientation.
    /// </summary>
    public abstract ListenerOrientation Orientation { get; set; }
}

