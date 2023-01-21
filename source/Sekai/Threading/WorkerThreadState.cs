// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Threading;

public enum WorkerThreadState
{
    /// <summary>
    /// The worker thread is idle.
    /// </summary>
    Idle,

    /// <summary>
    /// The worker thread is running.
    /// </summary>
    Active,

    /// <summary>
    /// The worker thread is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// THe worker thread has exited.
    /// </summary>
    Exited,
}
