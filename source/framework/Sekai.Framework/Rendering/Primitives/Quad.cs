// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sekai.Framework.Rendering.Primitives;

/// <summary>
/// A quadtrilaterial.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Quad : IPolygon
{
    /// <summary>
    /// The top left corner of the quad.
    /// </summary>
    public readonly Vector2 TopLeft;

    /// <summary>
    /// The bottom left corner of the quad.
    /// </summary>
    public readonly Vector2 BottomLeft;

    /// <summary>
    /// The bottom right corner of the quad.
    /// </summary>
    public readonly Vector2 BottomRight;

    /// <summary>
    /// The top right corner of the quad.
    /// </summary>
    public readonly Vector2 TopRight;

    public Quad(Vector2 topLeft, Vector2 bottomLeft, Vector2 bottomRight, Vector2 topRight)
    {
        TopLeft = topLeft;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
        TopRight = topRight;
    }

    public ReadOnlySpan<Vector2> GetVertices() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in TopLeft), 4);
}
