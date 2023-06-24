// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Headless.Audio;
using Sekai.Headless.Graphics;
using Sekai.Hosting;
using Sekai.Windowing;

namespace Sekai.Headless;

internal class DummyProvider : IAudioProvider, IGraphicsProvider, IPlatformProvider
{
    public AudioDevice CreateAudio()
    {
        return new DummyAudioDevice();
    }

    public GraphicsDevice CreateGraphics(IWindow window)
    {
        return new DummyGraphicsDevice();
    }

    public Platform CreatePlatform()
    {
        return new DummyPlatform();
    }
}
