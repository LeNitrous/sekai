// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Shaders;

namespace Sekai.OpenGL;

internal class GLShaderTranspiler : ShaderTranspiler
{
    private int textureCount = -1;

    protected override string Header { get; } = @"
#version 330 core
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
";

    protected override string FormatUniform(string type, string name, ShaderStage? stage = null)
    {
        if (!stage.HasValue)
            textureCount = -1;

        if (type.Contains("sampler") || type.Contains("image"))
        {
            textureCount++;
            return $"layout(binding = {textureCount}) uniform {type} {name};";
        }

        return base.FormatUniform(type, name, stage);
    }
}
