// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Audio;

namespace Sekai.Platform.Headless.Audio;

internal sealed class DummyAudioListener : AudioListener
{
    public override float Volume { get; set; }
    public override Vector3 Position { get; set; }
    public override Vector3 Velocity { get; set; }
    public override ListenerOrientation Orientation { get; set; }

    public override void Dispose()
    {
    }
}
