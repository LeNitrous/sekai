// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Audio.Dummy;

internal sealed class DummyAudioBuffer : AudioBuffer
{
    public override int Capacity { get; }
    public override int SampleRate { get; }
    public override AudioFormat Format { get; }

    public override void Dispose()
    {
    }

    public override void SetData(nint data, uint size, AudioFormat format, int sampleRate)
    {
    }
}
