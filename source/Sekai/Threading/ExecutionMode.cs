// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Threading;

/// <summary>
/// Determines how the thread controller should execute threads.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// All frames run in their own threads.
    /// </summary>
    MultiThread,

    /// <summary>
    /// All frames run in the main thread.
    /// </summary>
    SingleThread,
}
