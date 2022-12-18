// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Rendering;

/// <summary>
/// Determines how a given <see cref="Drawable"/> should be culled from rendering.
/// </summary>
public enum CullingMode
{
    /// <summary>
    /// The <see cref="Drawable"/> are culled using the <see cref="Camera"/>'s frustum.
    /// </summary>
    Frustum,

    /// <summary>
    /// The <see cref="Drawable"/> is never culled.
    /// </summary>
    None,
}
