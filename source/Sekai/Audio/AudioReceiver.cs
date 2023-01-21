// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Allocation;
using Sekai.Rendering;
using Sekai.Scenes;

namespace Sekai.Audio;

/// <summary>
/// A component that receives audio output.
/// </summary>
public class AudioReceiver : Component, IAudioListener
{
    /// <summary>
    /// Gets whether this receiver is the current.
    /// </summary>
    public bool IsCurrent => ReferenceEquals(audio.Current, this);

    [Bind]
    private Transform2D? transform2D { get; set; }

    [Bind]
    private Transform3D? transform3D { get; set; }

    [Resolved]
    private AudioContext audio { get; set; } = null!;

    /// <summary>
    /// Makes this receiver active.
    /// </summary>
    public void MakeCurrent()
    {
        audio.MakeCurrent(this);
    }

    protected override void Destroy()
    {
        audio.ClearCurrent(this);
        base.Destroy();
    }

    Vector3 IAudioListener.Position
    {
        get
        {
            if (transform2D is not null)
                return new Vector3(transform2D.Position, 0);

            return transform3D?.Position ?? Vector3.Zero;
        }
    }

    Vector3 IAudioListener.Velocity { get; }

    ListenerOrientation IAudioListener.Orientation { get; }
}
