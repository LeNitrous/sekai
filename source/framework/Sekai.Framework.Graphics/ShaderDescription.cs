// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct ShaderDescription : IEquatable<ShaderDescription>
{
    /// <summary>
    /// The entry point function name.
    /// </summary>
    public string EntryPoint;

    /// <summary>
    /// The shader stage this shader will use.
    /// </summary>
    public ShaderStage Stage;

    /// <summary>
    /// The shader code this will use.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>For Direct3D11 shaders, this array must contain HLSL bytecode or HLSL text.</item>
    /// <item>For Vulkan shaders, this array must contain SPIR-V bytecode.</item>
    /// <item>For OpenGL and OpenGL ES shaders, this array must contain the ASCII-encoded text of the shader code.</item>
    /// <item>For Metal shaders, this array must contain Metal bitcode (a "metallib" file), or UTF8-encoded Metal shading language text.</item>
    /// </list>
    /// </remarks>
    public byte[] Code;

    public override bool Equals(object? obj)
    {
        return obj is ShaderDescription description && Equals(description);
    }

    public bool Equals(ShaderDescription other)
    {
        return EntryPoint == other.EntryPoint &&
               Stage == other.Stage &&
               EqualityComparer<byte[]>.Default.Equals(Code, other.Code);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EntryPoint, Stage, Code);
    }

    public static bool operator ==(ShaderDescription left, ShaderDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ShaderDescription left, ShaderDescription right)
    {
        return !(left == right);
    }
}
