// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Reflection;
using Sekai.Framework.Graphics;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

/// <summary>
/// Options to customize initialization of <see cref="Host"/>.
/// </summary>
public class HostOptions
{
    /// <summary>
    /// The title of the game window.
    /// </summary>
    public string Title { get; set; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "Sekai Framework";

    /// <summary>
    /// Arguments obtained from launching the process.
    /// </summary>
    public string[] Arguments { get; set; } = Array.Empty<string>();

    /// <inheritdoc cref="FrameworkThreadManager.ExecutionMode"/>
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.MultiThread;

    /// <inheritdoc cref="FrameworkThreadManager.UpdatePerSecond"/>
    public double UpdatePerSecond { get; set; } = 240;

    /// <inheritdoc cref="FrameworkThreadManager.FramesPerSecond"/>
    public double FramesPerSecond { get; set; } = 120;

    /// <summary>
    /// The backend graphics to be used by the renderer.
    /// </summary>
    public GraphicsAPI Renderer { get; set; } = GraphicsAPI.Vulkan;
}
