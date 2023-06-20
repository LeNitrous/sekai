// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Framework.Mathematics;
using Sekai.Framework.Windowing;

namespace Sekai.Desktop.Windowing;

internal readonly unsafe struct Monitor : IMonitor
{
    public string Name { get; }
    public int Index { get; }
    public Point Position { get; }
    public VideoMode Mode { get; }
    public Silk.NET.GLFW.Monitor* Handle { get; }

    private readonly IEnumerable<VideoMode> modes;

    public Monitor(int index, string name, Silk.NET.GLFW.Monitor* handle, Point position, VideoMode mode, IEnumerable<VideoMode> modes)
    {
        Mode = mode;
        Name = name;
        Index = index;
        Handle = handle;
        Position = position;
        this.modes = modes;
    }

    public IEnumerable<VideoMode> GetSupportedVideoModes() => modes;
}
