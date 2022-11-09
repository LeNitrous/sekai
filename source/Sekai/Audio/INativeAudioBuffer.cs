// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Audio;

public interface INativeAudioBuffer : IDisposable
{
    void SetData(nint data, int size, AudioFormat format, int sampleRate);
}
