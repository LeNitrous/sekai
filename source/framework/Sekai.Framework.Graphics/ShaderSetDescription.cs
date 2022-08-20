// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct ShaderSetDescription : IEquatable<ShaderSetDescription>
{
    public VertexLayoutDescription[] Layouts;
    public IShader[] Shaders;
    public ShaderConstant[] Constants;

    public ShaderSetDescription(VertexLayoutDescription[] layouts, IShader[] shaders, ShaderConstant[] constants)
    {
        Layouts = layouts;
        Shaders = shaders;
        Constants = constants;
    }

    public override bool Equals(object? obj)
    {
        return obj is ShaderSetDescription description && Equals(description);
    }

    public bool Equals(ShaderSetDescription other)
    {
        return EqualityComparer<VertexLayoutDescription[]>.Default.Equals(Layouts, other.Layouts) &&
               EqualityComparer<IShader[]>.Default.Equals(Shaders, other.Shaders) &&
               EqualityComparer<ShaderConstant[]>.Default.Equals(Constants, other.Constants);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Layouts, Shaders, Constants);
    }

    public static bool operator ==(ShaderSetDescription left, ShaderSetDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ShaderSetDescription left, ShaderSetDescription right)
    {
        return !(left == right);
    }
}
