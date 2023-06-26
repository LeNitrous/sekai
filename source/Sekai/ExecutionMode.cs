// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai;

/// <summary>
/// An enumeration of execution modes.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// Executes the game loop in a single thread.
    /// </summary>
    SingleThread,

    /// <summary>
    /// Executes the game loop in multiple threads.
    /// </summary>
    MultiThread,
}
