// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Mathematics;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// A vertex that can be textured.
/// </summary>
public interface ITexturedVertex : IVertex
{
    /// <summary>
    /// The vertex texture coordinates.
    /// </summary>
    Vector2 TexCoord { get; set; }
}
