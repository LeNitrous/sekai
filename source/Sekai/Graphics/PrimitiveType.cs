// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// Input primitive types used for rasterization.
/// </summary>
public enum PrimitiveType
{
    /// <summary>
    /// Vertices are a triangle list.
    /// </summary>
    TriangleList,

    /// <summary>
    /// Vertices are a triangle strip.
    /// </summary>
    TriangleStrip,

    /// <summary>
    /// Vertices are in a line list.
    /// </summary>
    LineList,

    /// <summary>
    /// Vertices are in a line strip.
    /// </summary>
    LineStrip,

    /// <summary>
    /// Vertices are in a point list.
    /// </summary>
    PointList,

    /// <summary>
    /// Vertices are a triangle list, with adjacency.
    /// </summary>
    TriangleListAdjacency,

    /// <summary>
    /// Vertices are a triangle strip, with adjacency.
    /// </summary>
    TriangleStripAdjacency,

    /// <summary>
    /// Vertices are a line list, with adjacency.
    /// </summary>
    LineListAdjacency,

    /// <summary>
    /// Vertices are a line strip, with adjacency.
    /// </summary>
    LineStripAdjacency,
}
