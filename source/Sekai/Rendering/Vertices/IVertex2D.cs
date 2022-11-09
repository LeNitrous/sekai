// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Rendering.Vertices;

/// <summary>
/// A two-dimensional vertex.
/// </summary>
public interface IVertex2D : IVertex
{
    /// <summary>
    /// The vertex position.
    /// </summary>
    Vector2 Position { get; set; }
}
