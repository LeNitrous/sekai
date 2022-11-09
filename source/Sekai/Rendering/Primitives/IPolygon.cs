// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A two-dimensional figure.
/// </summary>
public interface IPolygon
{
    /// <summary>
    /// Gets the vertices in clip-space coordinates that define this polygon in counter-clockwise orientation.
    /// </summary>
    /// <returns>The vertices of this polygon.</returns>
    ReadOnlySpan<Vector2> GetVertices();
}
