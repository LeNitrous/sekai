// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines how a sequence of vertices is interpreted by the rasterizer.
/// </summary>
public enum PrimitiveTopology
{
    /// <summary>
    /// A list of isolated, 3-element triangles.
    /// </summary>
    Triangles,

    /// <summary>
    /// A series of connected triangles.
    /// </summary>
    TriangleStrip,

    /// <summary>
    /// A series of isolated, 2-element segments.
    /// </summary>
    Lines,

    /// <summary>
    /// A series of connected line segments.
    /// </summary>
    LineStrip,

    /// <summary>
    /// A series of isolated points.
    /// </summary>
    Points,
}
