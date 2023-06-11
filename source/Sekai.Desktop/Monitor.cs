// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Drawing;
using Sekai.Platform;

namespace Sekai.Desktop;

internal readonly struct Monitor : IMonitor
{
    public string Name { get; }
    public int Index { get; }
    public Rectangle Bounds { get; }
    public VideoMode Mode { get; }
    private readonly IEnumerable<VideoMode> supportedVideoModes;

    public Monitor(string name, int index, Rectangle bounds, VideoMode mode, IEnumerable<VideoMode> supportedVideoModes)
    {
        Mode = mode;
        Name = name;
        Index = index;
        Bounds = bounds;
        this.supportedVideoModes = supportedVideoModes;
    }

    public IWindow CreateWindow() => new Window();

    public IEnumerable<VideoMode> GetSupportedVideoModes() => supportedVideoModes;
}
