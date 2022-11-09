// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A three-dimensional figure.
/// </summary>
public interface IPolyhedron
{
    /// <summary>
    /// Gets the vertices in clip-space coordinates that define this polyhedron in counter-clockwise orientation.
    /// </summary>
    /// <returns>The vertices of this polyhedron.</returns>
    ReadOnlySpan<Vector3> GetVertices();
}
