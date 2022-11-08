// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Framework.Graphics.Vertices;

/// <summary>
/// A vertex that has a position in two-dimensional space.
/// </summary>
public interface IVertex2D : IVertex
{
    /// <summary>
    /// The vertex position.
    /// </summary>
    Vector2 Position { get; set; }
}
