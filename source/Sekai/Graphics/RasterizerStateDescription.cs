// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

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
    public bool Scissor;

    public RasterizerStateDescription(FaceCulling culling, FaceWinding winding, FillMode mode, bool scissorTest)
    {
        Culling = culling;
        Winding = winding;
        Mode = mode;
        Scissor = scissorTest;
    }

    public readonly bool Equals(RasterizerStateDescription other)
    {
        return Culling == other.Culling &&
               Winding == other.Winding &&
               Mode == other.Mode &&
               Scissor == other.Scissor;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is RasterizerStateDescription other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Culling, Winding, Mode, Scissor);
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
