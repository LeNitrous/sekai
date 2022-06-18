// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Threading;

/// <summary>
/// Tells how the threading manager should execute threads.
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
