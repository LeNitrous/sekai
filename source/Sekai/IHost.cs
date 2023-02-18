// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

public interface IHost
{
    /// <summary>
    /// Invoked when an exception has been thrown. Return true to continue execution.
    /// </summary>
    event Func<Exception, bool>? OnException;

    /// <summary>
    /// How threads will execute.
    /// </summary>
    ExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// How often updates will happen per second.
    /// </summary>
    double UpdatePerSecond { get; set; }

    /// <summary>
    /// Arguments obtained from launching the process.
    /// </summary>
    string[] Arguments { get; }

    /// <summary>
    /// Environment variables obtained from launching the process.
    /// </summary>
    IDictionary Variables { get; }

    /// <summary>
    /// The host's logger.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// The host's storage.
    /// </summary>
    Storage Storage { get; }

    /// <summary>
    /// The host's surface.
    /// </summary>
    Surface Surface { get; }

    /// <summary>
    /// The host's audio context.
    /// </summary>
    AudioContext Audio { get; }

    /// <summary>
    /// The host's input context.
    /// </summary>
    InputContext Input { get; }

    /// <summary>
    /// The host's graphics context.
    /// </summary>
    GraphicsContext Graphics { get; }

    /// <summary>
    /// Exits the currently executing host.
    /// </summary>
    void Exit();
}
