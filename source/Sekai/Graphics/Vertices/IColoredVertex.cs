// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Mathematics;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// A vertex that can be colored.
/// </summary>
public interface IColoredVertex : IVertex
{
    /// <summary>
    /// The vertex color.
    /// </summary>
    Color4 Color { get; set; }
}
