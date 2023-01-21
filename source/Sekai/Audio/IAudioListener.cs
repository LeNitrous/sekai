// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Audio;

public interface IAudioListener
{
    bool IsCurrent { get; }

    Vector3 Position { get; }

    Vector3 Velocity { get; }

    ListenerOrientation Orientation { get; }
}
