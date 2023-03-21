// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Platform;

/// <summary>
/// The interface that implements platform-specific routines in managing the lifecycle of a <see cref="Game"/>.
/// </summary>
public interface IGameHost
{
    /// <summary>
    /// The game host's services.
    /// </summary>
    IServiceLocator Services { get; }

    /// <summary>
    /// Called when the game host is being created.
    /// </summary>
    event Action? Create;

    /// <summary>
    /// Called when the game host is loading.
    /// </summary>
    event Action? Load;

    /// <summary>
    /// Called when the game host is updating.
    /// </summary>
    event Action? Tick;

    /// <summary>
    /// Called when the game host is pausing.
    /// </summary>
    event Action? Paused;

    /// <summary>
    /// Called when the game host is resuming.
    /// </summary>
    event Action? Resumed;

    /// <summary>
    /// Called when the game host is unloading.
    /// </summary>
    event Action? Unload;

    /// <summary>
    /// Called when the game host is being destroyed.
    /// </summary>
    event Action? Destroy;
}
