// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Describes a <see cref="RasterizerState"/>.
/// </summary>
public struct RasterizerStateDescription : IEquatable<RasterizerStateDescription>
{
    /// <summary>
    /// The face to cull.
    /// </summary>
    public FaceCulling Culling;

    /// <summary>
    /// The winding order of the vertices.
    /// </summary>
    public FaceWinding Winding;

    /// <summary>
    /// The fill mode of primitives.
    /// </summary>
    public FillMode Mode;

    /// <summary>
    /// Whether to enable or disable scissor testing.
    /// </summary>
    public bool ScissorTest;

    public RasterizerStateDescription(FaceCulling culling, FaceWinding winding, FillMode mode, bool scissorTest)
    {
        Culling = culling;
        Winding = winding;
        Mode = mode;
        ScissorTest = scissorTest;
    }

    public bool Equals(RasterizerStateDescription other)
    {
        return Culling == other.Culling &&
               Winding == other.Winding &&
               Mode == other.Mode &&
               ScissorTest == other.ScissorTest;
    }

    public override bool Equals(object? obj)
    {
        return obj is RasterizerStateDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Culling, Winding, Mode, ScissorTest);
    }

    public static bool operator ==(RasterizerStateDescription left, RasterizerStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RasterizerStateDescription left, RasterizerStateDescription right)
    {
        return !(left == right);
    }
}
