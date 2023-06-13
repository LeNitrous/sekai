// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Storages;
using System;

namespace Sekai.Platform;

/// <summary>
/// Hosts a <see cref="Game"/> and performs its lifecycle events.
/// </summary>
public interface IHost
{
    /// <summary>
    /// Called every frame.
    /// </summary>
    event Action? Tick;

    /// <summary>
    /// Called when the host pauses execution.
    /// </summary>
    event Action? Paused;

    /// <summary>
    /// Called when the host resumes execution.
    /// </summary>
    event Action? Resumed;

    /// <summary>
    /// The host's window.
    /// </summary>
    IWindow? Window { get; }

    /// <summary>
    /// The host's storage.
    /// </summary>
    Storage? Storage { get; }

    /// <summary>
    /// The host's state.
    /// </summary>
    HostState State { get; }

    /// <summary>
    /// Exits the host.
    /// </summary>
    void Exit();
}
