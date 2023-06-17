// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Drawing;
using Sekai.Platform.Windowing;

namespace Sekai.Platform.Desktop;

public readonly struct Monitor : IMonitor
{
    public string Name { get; }
    public int Index { get; }
    public Point Position { get; }
    public VideoMode Mode { get; }

    private readonly IEnumerable<VideoMode> modes;

    public Monitor(int index, string name, Point position, VideoMode mode, IEnumerable<VideoMode> modes)
    {
        Mode = mode;
        Name = name;
        Index = index;
        Position = position;
        this.modes = modes;
    }

    public IEnumerable<VideoMode> GetSupportedVideoModes() => modes;
}
