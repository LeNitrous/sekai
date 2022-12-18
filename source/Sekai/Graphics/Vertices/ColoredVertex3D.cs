// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Mathematics;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// A vertex in three-dimensional space that can be colored.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ColoredVertex3D : IVertex3D, IColoredVertex
{
    [field: VertexMember("Position", 3)]
    public Vector3 Position { get; set; }

    [field: VertexMember("Color", 4)]
    public Color4 Color { get; set; }
}
