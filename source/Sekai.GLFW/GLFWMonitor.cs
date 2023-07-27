// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.GLFW;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
internal sealed unsafe class GLFWMonitor : Monitor
{
    public override int Index { get; }

    public override string Name => glfw.GetMonitorName(Handle);

    public override bool Primary => Index == 0;

    public override VideoMode Mode
    {
        get
        {
            var mode = glfw.GetVideoMode(Handle);
            return new(new(mode->Width, mode->Height), mode->RefreshRate);
        }
    }

    public Point Position
    {
        get
        {
            glfw.GetMonitorPos(Handle, out int x, out int y);
            return new(x, y);
        }
    }

    public Silk.NET.GLFW.Monitor* Handle;

    private VideoMode[] modes = Array.Empty<VideoMode>();
    private readonly Silk.NET.GLFW.Glfw glfw;

    public GLFWMonitor(Silk.NET.GLFW.Glfw glfw, int index)
    {
        Index = index;
        this.glfw = glfw;
    }

    public override IWindow CreateWindow()
    {
        return new GLFWWindow(glfw);
    }

    public override IEnumerable<VideoMode> GetVideoModes()
    {
        var modesHandle = glfw.GetVideoModes(Handle, out int count);

        if (modes.Length <= count)
        {
            Array.Resize(ref modes, count);
        }

        for (int i = 0; i < modes.Length; i++)
        {
            var mode = modesHandle[i];
            modes[i] = new(new(mode.Width, mode.Height), mode.RefreshRate);
        }

        return modes;
    }
}
