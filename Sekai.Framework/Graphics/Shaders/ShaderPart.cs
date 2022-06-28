// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Extensions;
using Veldrid.SPIRV;

namespace Sekai.Framework.Graphics.Shaders;

public class ShaderPart : IEquatable<ShaderPart>
{
    /// <summary>
    /// The SPIR-V bytecode of this shader part.
    /// </summary>
    public readonly byte[] ByteCode;

    /// <summary>
    /// The stages this shader part will be used on.
    /// </summary>
    public readonly ShaderStage Stage;

    /// <summary>
    /// Construct a shader part using GLSL shader code.
    /// </summary>
    public ShaderPart(string shaderCode, ShaderStage stage)
        : this(SpirvCompilation.CompileGlslToSpirv(shaderCode, null, stage.ToVeldrid(), compileOptions).SpirvBytes, stage)
    {
    }

    /// <summary>
    /// Construct a shader part using compiled SPIR-V byte code.
    /// </summary>
    public ShaderPart(byte[] byteCode, ShaderStage stage)
    {
        if (!isValid(byteCode))
            throw new ArgumentException($"{nameof(byteCode)} is not valid SPIR-V byte code.");

        Stage = stage;
        ByteCode = byteCode;
    }

    private static readonly GlslCompileOptions compileOptions = new(true);

    private static bool isValid(byte[] byteCode)
    {
        return byteCode.Length > 4
            && byteCode[0] == 0x03
            && byteCode[1] == 0x02
            && byteCode[2] == 0x23
            && byteCode[3] == 0x07;
    }

    public bool Equals(ShaderPart? other)
    {
        return other is not null
            && Stage == other.Stage
            && ByteCode.SequenceEqual(other.ByteCode);
    }

    public override bool Equals(object? obj) => Equals(obj as ShaderPart);

    public override int GetHashCode() => HashCode.Combine(ByteCode, Stage);
}
