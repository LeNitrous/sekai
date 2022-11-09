// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// Determines which face will be culled.
/// </summary>
public enum FaceCulling
{
    /// <summary>
    /// The back face is culled.
    /// </summary>
    Back,
    /// <summary>
    /// The front face is culled.
    /// </summary>
    Front,
    /// <summary>
    /// No face culling.
    /// </summary>
    None,
}
