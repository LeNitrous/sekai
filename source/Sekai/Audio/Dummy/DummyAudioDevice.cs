// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Audio.Dummy;

internal sealed class DummyAudioDevice : AudioDevice
{
    public override AudioListener Listener { get; } = new DummyAudioListener();

    public override AudioBuffer CreateBuffer()
    {
        return new DummyAudioBuffer();
    }

    public override AudioSource CreateSource()
    {
        return new DummyAudioSource();
    }

    public override void Dispose()
    {
    }
}
