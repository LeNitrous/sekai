// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics.Textures;

/// <summary>
/// Determines how textures are sampled beyond its coordinate range.
/// </summary>
public enum WrapMode
{
    /// <summary>
    /// No wrapping.
    /// </summary>
    None,

    /// <summary>
    /// Repeats the texture.
    /// </summary>
    Repeat,

    /// <summary>
    /// Repeats the texture but the adjacent is mirrored.
    /// </summary>
    RepeatMirrored,

    /// <summary>
    /// Clamps to the edge of the texture repeating the edge to fill the remaining area.
    /// </summary>
    ClampToEdge,

    /// <summary>
    /// Clamps to a transparent border repeating the border to fill the remaining area.
    /// </summary>
    ClampToBorder,
}
