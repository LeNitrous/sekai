// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.InteropServices;
using Sekai.Mathematics;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// A vertex in two-dimensional space that can be textured and colored.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TexturedVertex2D : IVertex2D, ITexturedVertex, IColoredVertex
{
    [field: VertexMember("Position", 2)]
    public Vector2 Position { get; set; }

    [field: VertexMember("TexCoord", 2)]
    public Vector2 TexCoord { get; set; }

    [field: VertexMember("Color", 4)]
    public Color4 Color { get; set; }
}
