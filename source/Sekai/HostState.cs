// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai;

/// <summary>
/// Enumeration of possible host states.
/// </summary>
public enum HostState
{
    /// <summary>
    /// The host is idle and is ready to run.
    /// </summary>
    Idle,

    /// <summary>
    /// The host is currently loading.
    /// </summary>
    Loading,

    /// <summary>
    /// The host is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// The host is restarting.
    /// </summary>
    Reloading,

    /// <summary>
    /// The host is pausing.
    /// </summary>
    Pausing,

    /// <summary>
    /// The host is currently paused.
    /// </summary>
    Paused,

    /// <summary>
    /// The host is resuming.
    /// </summary>
    Resuming,

    /// <summary>
    /// The host has resumed.
    /// </summary>
    Resumed,

    /// <summary>
    /// The host is exiting.
    /// </summary>
    Exiting,

    /// <summary>
    /// The host has exited.
    /// </summary>
    Exited,
}
