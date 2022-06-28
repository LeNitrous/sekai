// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines how the source and destination factors are combined in blend operations.
/// </summary>
public enum BlendFunction
{
    /// <summary>
    /// Source and destination are added.
    /// </summary>
    Add,

    /// <summary>
    /// Destination is subtracted from the source.
    /// </summary>
    Subtract,

    /// <summary>
    /// Source is subtracted from the destination.
    /// </summary>
    ReverseSubtract,

    /// <summary>
    /// The lower value between source and destination is selected.
    /// </summary>
    Minimum,

    /// <summary>
    /// The higher value between source and destination is selected.
    /// </summary>
    Maximum,
}
