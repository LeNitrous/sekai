// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// Options passed to <see cref="Host"/> during initialization.
/// </summary>
public sealed class HostOptions
{
    /// <summary>
    /// The game's name.
    /// </summary>
    public string Name { get; init; } = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "Sekai";

    /// <summary>
    /// The host's tick mode.
    /// </summary>
    public TickMode TickMode { get; init; } = TickMode.Variable;

    /// <summary>
    /// The host's execution mode.
    /// </summary>
    public ExecutionMode ExecutionMode { get; init; } = ExecutionMode.MultiThread;

    /// <summary>
    /// The host's update rate.
    /// </summary>
    public double UpdateRate { get; init; } = 120.0;

    /// <summary>
    /// The window's size.
    /// </summary>
    public Size WindowSize { get; init; } = new Size(1280, 720);

    /// <summary>
    /// The window's border presentation.
    /// </summary>
    public WindowBorder WindowBorder { get; init; } = WindowBorder.Resizable;

    /// <summary>
    /// The arguments passed when the program was launched.
    /// </summary>
    public IReadOnlyList<string> Arguments { get; } = Environment.GetCommandLineArgs();

    /// <summary>
    /// The environment variables passed when the program was launched.
    /// </summary>
    public IDictionary Variables { get; } = Environment.GetEnvironmentVariables();
}
