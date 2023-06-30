// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.Desktop.Windowing;

internal readonly unsafe struct Monitor : IMonitor
{
    public string Name { get; }
    public int Index { get; }
    public Point Position { get; }
    public VideoMode Mode { get; }
    public Silk.NET.GLFW.Monitor* Handle { get; }

    private readonly IEnumerable<VideoMode> modes;

    private Monitor(int index, string name, Silk.NET.GLFW.Monitor* handle, Point position, VideoMode mode, IEnumerable<VideoMode> modes)
    {
        Mode = mode;
        Name = name;
        Index = index;
        Handle = handle;
        Position = position;
        this.modes = modes;
    }

    public static Monitor From(int index, Silk.NET.GLFW.Glfw glfw, Silk.NET.GLFW.Monitor* handle)
    {
        glfw.GetMonitorPos(handle, out int x, out int y);

        var modes = glfw.GetVideoModes(handle, out int modeCount);
        var ms = new VideoMode[modeCount];
        var mc = glfw.GetVideoMode(handle);

        for (int i = 0; i < modeCount; i++)
        {
            ms[i] = new(new(modes[i].Width, modes[i].Height), modes[i].RefreshRate);
        }

        string name = glfw.GetMonitorName(handle);

        return new(index, name, handle, new(x, y), new(new(mc->Width, mc->Height), mc->RefreshRate), ms);
    }

    public IEnumerable<VideoMode> GetSupportedVideoModes() => modes;
}
