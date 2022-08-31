// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct GraphicsPipelineDescription : IEquatable<GraphicsPipelineDescription>
{
    /// <summary>
    /// A description of the blend state which controls how color values are blended into each color target.
    /// </summary>
    public BlendStateDescription Blend;

    /// <summary>
    /// A description of the rasterizer state which controls culling, clipping, scissor, and polygon-fill behavior.
    /// </summary>
    public RasterizerStateDescription Rasterizer;

    /// <summary>
    /// A description of the depth stencil state which controls depth tests, writes, and comparisons.
    /// </summary>
    public DepthStencilStateDescription DepthStencil;

    /// <summary>
    /// A description of the shader set to be used.
    /// </summary>
    public ShaderSetDescription ShaderSet;

    /// <summary>
    /// Determines how a series of input vertices is interpreted by the <see cref="IPipeline"/>.
    /// </summary>
    public PrimitiveTopology Topology;

    /// <summary>
    /// Determines the layout of shader resources used in the <see cref="IPipeline"/>.
    /// </summary>
    public IResourceLayout[] Layouts;

    /// <summary>
    /// A description of output attachments used by the <see cref="IPipeline"/>.
    /// </summary>
    public OutputDescription Outputs;

    public GraphicsPipelineDescription(BlendStateDescription blend, RasterizerStateDescription rasterizer, DepthStencilStateDescription depthStencil, ShaderSetDescription shaderSet, PrimitiveTopology topology, IResourceLayout[] layouts, OutputDescription outputs)
    {
        Blend = blend;
        Rasterizer = rasterizer;
        DepthStencil = depthStencil;
        ShaderSet = shaderSet;
        Topology = topology;
        Layouts = layouts;
        Outputs = outputs;
    }

    public override bool Equals(object? obj)
    {
        return obj is GraphicsPipelineDescription other && Equals(other);
    }

    public bool Equals(GraphicsPipelineDescription other)
    {
        return Blend.Equals(other.Blend) &&
               Rasterizer.Equals(other.Rasterizer) &&
               DepthStencil.Equals(other.DepthStencil) &&
               ShaderSet.Equals(other.ShaderSet) &&
               Topology == other.Topology &&
               Enumerable.SequenceEqual(Layouts, other.Layouts) &&
               Outputs.Equals(other.Outputs);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Blend, Rasterizer, DepthStencil, ShaderSet, Topology, Layouts, Outputs);
    }

    public static bool operator ==(GraphicsPipelineDescription left, GraphicsPipelineDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GraphicsPipelineDescription left, GraphicsPipelineDescription right)
    {
        return !(left == right);
    }
}
