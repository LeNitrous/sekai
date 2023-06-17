// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// The face to cull.
/// </summary>
public enum FaceCulling
{
    /// <summary>
    /// Disable face culling.
    /// </summary>
    None,

    /// <summary>
    /// Cull front faces.
    /// </summary>
    Front,

    /// <summary>
    /// Cull back faces.
    /// </summary>
    Back,
}
