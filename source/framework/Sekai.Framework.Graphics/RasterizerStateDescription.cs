// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct RasterizerStateDescription : IEquatable<RasterizerStateDescription>
{
    /// <summary>
    /// Controls which faces are culled.
    /// </summary>
    public FaceCulling Culling;

    /// <summary>
    /// Controls the winding order used to determine the front face of primitives.
    /// </summary>
    public FaceWinding Winding;

    /// <summary>
    /// Controls how the rasterizer fills polygons.
    /// </summary>
    public PolygonFillMode FillMode;

    /// <summary>
    /// Whether depth clipping is enabled.
    /// </summary>
    public bool DepthClip;

    /// <summary>
    /// Whether scissor testing is enabled.
    /// </summary>
    public bool ScissorTest;

    public RasterizerStateDescription(FaceCulling culling, FaceWinding winding, PolygonFillMode fillMode, bool depthClip, bool scissorTest)
    {
        Culling = culling;
        Winding = winding;
        FillMode = fillMode;
        DepthClip = depthClip;
        ScissorTest = scissorTest;
    }

    public override bool Equals(object? obj)
    {
        return obj is RasterizerStateDescription other && Equals(other);
    }

    public bool Equals(RasterizerStateDescription other)
    {
        return Culling == other.Culling &&
               Winding == other.Winding &&
               FillMode == other.FillMode &&
               DepthClip == other.DepthClip &&
               ScissorTest == other.ScissorTest;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Culling, Winding, FillMode, DepthClip, ScissorTest);
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
