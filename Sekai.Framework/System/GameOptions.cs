// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using System.Reflection;
using Sekai.Framework.Threading;
using Veldrid;

namespace Sekai.Framework.Systems;

public class GameOptions
{
    public static readonly GameOptions Default = new();

    public string Title { get; set; } = Assembly.GetEntryAssembly()?.GetName().Name ?? Assembly.GetExecutingAssembly().GetName().Name ?? @"Sekai Framework";
    public bool VSync { get; set; } = false;
    public Size Size { get; set; } = new Size(1366, 768);
    public double FramesPerSecond { get; set; } = 120;
    public double UpdatePerSecond { get; set; } = 240;
    public GraphicsBackend Backend { get; set; } = GraphicsBackend.Vulkan;
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.MultiThread;
}
