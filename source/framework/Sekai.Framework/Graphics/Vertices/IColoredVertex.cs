// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics.Vertices;

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
