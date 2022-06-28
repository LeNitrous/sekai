// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines how a series of vertices should be interpreted.
/// </summary>
public enum PrimitiveTopology
{
    /// <summary>
    /// A series of isolated triangles.
    /// </summary>
    TriangleList,

    /// <summary>
    /// A series of connected triangles.
    /// </summary>
    TriangleStrip,

    /// <summary>
    /// A series of isolated line segments.
    /// </summary>
    LineList,

    /// <summary>
    /// A series of connected line segments.
    /// </summary>
    LineStrip,

    /// <summary>
    /// A series of isolated points.
    /// </summary>
    PointList,
}
