// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai;

/// <summary>
/// An enumeration of game loop processing modes.
/// </summary>
public enum TickMode
{
    /// <summary>
    /// Fixed time step.
    /// </summary>
    /// <remarks>
    /// A tick occurs at a fixed time interval.
    /// </remarks>
    Fixed,

    /// <summary>
    /// Variable time step.
    /// </summary>
    /// <remarks>
    /// A tick occurs at a variable time interval.
    /// </remarks>
    Variable,
}
