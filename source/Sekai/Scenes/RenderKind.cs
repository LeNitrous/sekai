// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Scenes;

/// <summary>
/// Determines the kind of renderer to be used.
/// </summary>
internal enum RenderKind
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Two-dimensional renderable.
    /// </summary>
    Render2D,

    /// <summary>
    /// Three-dimensional renderer.
    /// </summary>
    Render3D,
}
