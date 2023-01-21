// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;

namespace Sekai.Null.Audio;

internal class NullAudioBuffer : NativeAudioBuffer
{
    public override int Size { get; }

    public override int SampleRate { get; }

    public override AudioFormat Format { get; }

    public override void SetData(nint data, int size, AudioFormat format, int sampleRate)
    {
    }
}
