// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Effects;

public class EffectCompiler : FrameworkObject
{
    private readonly Dictionary<ShaderStage, EffectTranspiler> transpilers = new()
    {
        { ShaderStage.Vertex, new EffectVertexTranspiler() },
        { ShaderStage.Compute, new EffectComputeTranspiler() },
        { ShaderStage.Fragment, new EffectFragmentTranspiler() }
    };

    public Effect Compile(IGraphicsDevice device, EffectSource source, EffectType type) => type switch
    {
        EffectType.Compute => compileComputeEffect(device, source),
        EffectType.Graphics => compileGraphicsEffect(device, source),
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private Effect compileGraphicsEffect(IGraphicsDevice device, EffectSource source)
    {
        var vert = transpilers[ShaderStage.Vertex].Transpile(source.Code);
        var frag = transpilers[ShaderStage.Fragment].Transpile(source.Code);
        var vertCompiled = device.CompileShader(vert.Code, ShaderStage.Vertex, new ShaderCompilationOptions(source.Name));
        var fragCompiled = device.CompileShader(frag.Code, ShaderStage.Fragment, new ShaderCompilationOptions(source.Name));
        return new Effect(device, source.Name, EffectType.Graphics, device.GraphicsAPI, vert.Stages, vert.Resources, vertCompiled.Code, fragCompiled.Code);
    }

    private Effect compileComputeEffect(IGraphicsDevice device, EffectSource source)
    {
        var comp = transpilers[ShaderStage.Compute].Transpile(source.Code);
        var compCompiled = device.CompileShader(comp.Code, ShaderStage.Compute, new ShaderCompilationOptions(source.Name));
        return new Effect(device, source.Name, EffectType.Compute, device.GraphicsAPI, comp.Stages, comp.Resources, compCompiled.Code);
    }
}
