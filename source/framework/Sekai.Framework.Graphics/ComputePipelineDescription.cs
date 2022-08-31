// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct ComputePipelineDescription : IEquatable<ComputePipelineDescription>
{
    /// <summary>
    /// The shader to be used in the pipeline. The shader must be a compute shader.
    /// </summary>
    public IShader Shader;

    /// <summary>
    /// A list of resource layouts which controls the layout of shader resources in the pipeline.
    /// </summary>
    public IResourceLayout[] Layouts;

    /// <summary>
    /// The X dimension of the thread group size.
    /// </summary>
    public uint ThreadGroupSizeX;

    /// <summary>
    /// The Y dimension of the thread group size.
    /// </summary>
    public uint ThreadGroupSizeY;

    /// <summary>
    /// The Z dimension of the thread group size.
    /// </summary>
    public uint ThreadGroupSizeZ;

    public ComputePipelineDescription(IShader shader, IResourceLayout[] layouts, uint sizeX, uint sizeY, uint sizeZ)
    {
        Shader = shader;
        Layouts = layouts;
        ThreadGroupSizeX = sizeX;
        ThreadGroupSizeY = sizeY;
        ThreadGroupSizeZ = sizeZ;
    }

    public override bool Equals(object? obj)
    {
        return obj is ComputePipelineDescription other && Equals(other);
    }

    public bool Equals(ComputePipelineDescription other)
    {
        return EqualityComparer<IShader>.Default.Equals(Shader, other.Shader) &&
               Enumerable.SequenceEqual(Layouts, other.Layouts) &&
               ThreadGroupSizeX == other.ThreadGroupSizeX &&
               ThreadGroupSizeY == other.ThreadGroupSizeY &&
               ThreadGroupSizeZ == other.ThreadGroupSizeZ;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Shader, Layouts, ThreadGroupSizeX, ThreadGroupSizeY, ThreadGroupSizeZ);
    }

    public static bool operator ==(ComputePipelineDescription left, ComputePipelineDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ComputePipelineDescription left, ComputePipelineDescription right)
    {
        return !(left == right);
    }
}
