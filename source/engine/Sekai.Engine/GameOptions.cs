// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using System.Reflection;
using Sekai.Framework.Graphics;
using Sekai.Framework.Threading;

namespace Sekai.Engine;

/// <summary>
/// Options to customize initialization of a <see cref="Host"/>.
/// </summary>
public class GameOptions
{
    /// <summary>
    /// The title of the game window.
    /// </summary>
    public string Title { get; set; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "Sekai Framework";

    /// <summary>
    /// The size of the game window.
    /// </summary>
    public Size Size { get; set; } = new Size(1280, 720);

    /// <summary>
    /// Arguments obtained from launching the process.
    /// </summary>
    public string[] Arguments { get; set; } = Array.Empty<string>();

    /// <inheritdoc cref="Threading.ThreadController.ExecutionMode"/>
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.MultiThread;

    /// <inheritdoc cref="Threading.ThreadController.UpdatePerSecond"/>
    public double UpdatePerSecond { get; set; } = 240;

    /// <inheritdoc cref="Threading.ThreadController.FramesPerSecond"/>
    public double FramesPerSecond { get; set; } = 120;

    /// <summary>
    /// The graphics options used by the <see cref="IGraphicsDevice"/>.
    /// </summary>
    public GraphicsContextOptions Graphics { get; set; } = new();
}
