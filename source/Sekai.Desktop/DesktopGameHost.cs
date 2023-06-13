// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.Versioning;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Platform;

namespace Sekai.Desktop;

/// <summary>
/// A game host that has a window.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
public class DesktopGameHost : Host
{
    protected override IWindow CreateWindow() => new SDLWindow();

    protected override AudioDevice CreateAudio() => AudioDevice.CreateAL();

    protected override GraphicsDevice CreateGraphics()
    {
        if (Window?.Surface is null)
        {
            return base.CreateGraphics();
        }

        return GraphicsDevice.Create(Window.Surface);
    }

    protected override InputSource CreateInput()
    {
        if (Window is not SDLWindow sdlWindow)
        {
            throw new InvalidOperationException("Failed to create input.");
        }

        return new SDLInputSource(sdlWindow);
    }
}
