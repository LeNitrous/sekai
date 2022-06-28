// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics.Shaders;

namespace Sekai.Framework.Graphics;

public struct GraphicsPipelineState : IEquatable<GraphicsPipelineState>, IPipeline
{
    public Shader Shader { get; set; }
    public DepthInfo Depth { get; set; }
    public OutputInfo Output { get; set; }
    public BlendInfo Blending { get; set; }
    public StencilInfo Stencil { get; set; }
    public RasterizerInfo Rasterizer { get; set; }

    public bool Equals(GraphicsPipelineState other)
    {
        return Shader.Equals(other.Shader)
            && Depth == other.Depth
            && Output == other.Output
            && Stencil == other.Stencil
            && Blending == other.Blending
            && Rasterizer == other.Rasterizer;
    }

    public override bool Equals(object? obj) => obj is GraphicsPipelineState state && Equals(state);

    public override int GetHashCode() => HashCode.Combine(Shader, Depth, Output, Blending, Stencil, Rasterizer);

    public static bool operator ==(GraphicsPipelineState left, GraphicsPipelineState right) => left.Equals(right);

    public static bool operator !=(GraphicsPipelineState left, GraphicsPipelineState right) => !(left == right);
}
