// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Audio;

namespace Sekai.Null.Audio;

internal class NullAudioListener : NativeAudioListener
{
    public override float Volume { get; set; } = 1.0f;
    public override Vector3 Position { get; set; }
    public override Vector3 Velocity { get ; set; }
    public override ListenerOrientation Orientation { get; set; }
}
