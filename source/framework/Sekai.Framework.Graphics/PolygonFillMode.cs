// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines how the rasterizer will fill polygons.
/// </summary>
public enum PolygonFillMode
{
    /// <summary>
    /// Polygons are filled completely.
    /// </summary>
    Solid,

    /// <summary>
    /// Polygones are outlined in a "wireframe" style.
    /// </summary>
    Wireframe,
}
