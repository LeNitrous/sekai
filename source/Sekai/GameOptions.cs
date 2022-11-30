// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Reflection;
using Sekai.Mathematics;

namespace Sekai;

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
    public Size2 Size { get; set; } = new Size2(1280, 720);

    /// <summary>
    /// Arguments obtained from launching the process.
    /// </summary>
    public string[] Arguments { get; } = Environment.GetCommandLineArgs();

    /// <summary>
    /// Environment variables obtained from launching the process.
    /// </summary>
    public IDictionary Variables { get; } = Environment.GetEnvironmentVariables();
}
