// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Mathematics;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// A vertex in two-dimensional space that can be colored.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ColoredVertex2D : IVertex2D, IColoredVertex
{
    [field: VertexMember("Position", 2)]
    public Vector2 Position { get; set; }

    [field: VertexMember("Color", 4)]
    public Color4 Color { get; set; }
}
