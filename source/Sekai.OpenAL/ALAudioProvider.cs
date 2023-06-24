// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;
using Sekai.Hosting;

namespace Sekai.OpenAL;

internal sealed class ALAudioProvider : IAudioProvider
{
    public AudioDevice CreateAudio()
    {
        return new ALAudioDevice();
    }
}
