// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public readonly struct ShaderCompilationResult
{
    /// <summary>
    /// The filename of the compiled shader.
    /// </summary>
    public readonly string Filename;

    /// <summary>
    /// The translated shader code.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>For Direct3D11 shaders, this array contains HLSL bytecode or HLSL text.</item>
    /// <item>For Vulkan shaders, this array contains SPIR-V bytecode.</item>
    /// <item>For OpenGL and OpenGL ES shaders, this array contains the ASCII-encoded text of the shader code.</item>
    /// <item>For Metal shaders, this array contains UTF8-encoded Metal shading language text.</item>
    /// </list>
    /// </remarks>
    public readonly byte[] Code;

    /// <summary>
    /// The SPIR-V bytes of this shader code.
    /// </summary>
    public readonly byte[] Bytes;

    /// <summary>
    /// The resulting reflection data.
    /// </summary>
    public readonly ShaderReflectionResult Reflection;

    /// <summary>
    /// The shader stage.
    /// </summary>
    public readonly ShaderStage Stage;

    /// <summary>
    /// Exception thrown during compilation.
    /// </summary>
    public readonly Exception? Exception;

    public ShaderCompilationResult(string filename, byte[] code, byte[] bytes, ShaderStage stage, ShaderReflectionResult reflection, Exception? exception)
    {
        Code = code;
        Bytes = bytes;
        Stage = stage;
        Filename = filename;
        Exception = exception;
        Reflection = reflection;
    }
}
