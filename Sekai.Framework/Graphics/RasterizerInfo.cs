// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Information determining how the rasterizer should behave.
/// </summary>
public struct RasterizerInfo : IEquatable<RasterizerInfo>
{
    /// <summary>
    /// The default rasterization info.
    /// </summary>
    public static readonly RasterizerInfo Default = new(PrimitiveTopology.TriangleStrip, FaceCulling.Back, FaceWinding.Clockwise, PolygonFillMode.Solid, true, false);

    /// <summary>
    /// How a series of vertices should be interpreted.
    /// </summary>
    public readonly PrimitiveTopology Topology;

    /// <summary>
    /// How faces will be culled.
    /// </summary>
    public readonly FaceCulling Culling;

    /// <summary>
    /// How the vertices will be ordered.
    /// </summary>
    public readonly FaceWinding Winding;

    /// <summary>
    /// How rasterized polygons will be filled.
    /// </summary>
    public readonly PolygonFillMode FillMode;

    /// <summary>
    /// Whether depth clipping should be enabled.
    /// </summary>
    public readonly bool DepthClip;

    /// <summary>
    /// Whhether scissor tests should be performed.
    /// </summary>
    public readonly bool ScissorTest;

    public RasterizerInfo(PrimitiveTopology topology, FaceCulling culling, FaceWinding winding, PolygonFillMode fillMode, bool depthClip, bool scissorTest)
    {
        Culling = culling;
        Winding = winding;
        FillMode = fillMode;
        Topology = topology;
        DepthClip = depthClip;
        ScissorTest = scissorTest;
    }

    public bool Equals(RasterizerInfo other)
    {
        return Culling == other.Culling
            && Winding == other.Winding
            && FillMode == other.FillMode
            && Topology == other.Topology
            && DepthClip == other.DepthClip
            && ScissorTest == other.ScissorTest;
    }

    public override bool Equals(object? obj) => obj is RasterizerInfo info && Equals(info);

    public override int GetHashCode() => HashCode.Combine(Topology, Culling, Winding, FillMode, DepthClip, ScissorTest);

    public static bool operator ==(RasterizerInfo left, RasterizerInfo right) => left.Equals(right);

    public static bool operator !=(RasterizerInfo left, RasterizerInfo right) => !(left == right);
}
