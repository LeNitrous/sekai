// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;

namespace Sekai.Platform.Headless.Audio;

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
