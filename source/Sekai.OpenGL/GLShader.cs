// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Graphics.Shaders;

namespace Sekai.OpenGL;

internal class GLShader : NativeShader
{
    public override ShaderType Type => shaderIds.Length == 2 ? ShaderType.Graphic : ShaderType.Compute;
    public override IReadOnlyList<IUniform> Uniforms { get; }

    private readonly uint programId;
    private readonly uint[] shaderIds;
    private readonly GLGraphicsSystem system;

    public GLShader(GLGraphicsSystem system, uint programId, uint[] shaderIds, IReadOnlyList<IUniform> uniforms)
    {
        Uniforms = uniforms;
        this.system = system;
        this.programId = programId;
        this.shaderIds = shaderIds;
    }

    protected override void Dispose(bool disposing)
    {
        system.DestroyShader(programId, shaderIds);
    }

    public static implicit operator uint(GLShader shader) => shader.programId;
}
