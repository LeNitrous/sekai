// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Controls how the source and destination factors are combined in a blend operation.
/// </summary>
public enum BlendFunction
{
    /// <summary>
    /// Source and destination are added.
    /// </summary>
    Add,

    /// <summary>
    /// Destination is subtracted from source.
    /// </summary>
    Subtract,

    /// <summary>
    /// Source is subtracted from destination.
    /// </summary>
    ReverseSubtract,

    /// <summary>
    /// The minimum of source and destination are selected.
    /// </summary>
    Minimum,

    /// <summary>
    /// The maximum of source and destination are selected.
    /// </summary>
    Maximum,
}
