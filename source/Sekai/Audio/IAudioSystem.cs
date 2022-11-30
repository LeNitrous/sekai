// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Audio;

public interface IAudioSystem : IDisposable
{
    /// <summary>
    /// Creates an audio buffer.
    /// </summary>
    INativeAudioBuffer CreateBuffer();
}
