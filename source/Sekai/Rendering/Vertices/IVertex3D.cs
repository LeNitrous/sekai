// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Rendering.Vertices;

/// <summary>
/// A three-dimensional vertex.
/// </summary>
public interface IVertex3D : IVertex
{
    /// <summary>
    /// The vertex position.
    /// </summary>
    Vector3 Position { get; set; }
}
