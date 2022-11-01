// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using Sekai.Framework.Threading;

namespace Sekai.Framework;

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
    public string[] Arguments { get; } = Environment.GetCommandLineArgs();

    /// <summary>
    /// Environment variables obtained from launching the process.
    /// </summary>
    public IDictionary Variables { get; } = Environment.GetEnvironmentVariables();

    /// <inheritdoc cref="ThreadController.ExecutionMode"/>
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.MultiThread;

    /// <inheritdoc cref="ThreadController.UpdatePerSecond"/>
    public double UpdatePerSecond { get; set; } = 240;
}
