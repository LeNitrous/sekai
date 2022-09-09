// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Engine.Effects.Documents;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Effects.Compiler;

public class EffectCompiler : FrameworkObject
{
    private readonly IGraphicsDevice device;
    private readonly EffectVertexTranspiler vertTranspiler = new();
    private readonly EffectComputeTranspiler compTranspiler = new();
    private readonly EffectFragmentTranspiler fragTranspiler = new();

    public EffectCompiler(IGraphicsDevice device)
    {
        this.device = device;
    }

    public Effect Compile(EffectSource source, EffectType type)
    {
        var document = EffectDocument.Load(source.Code);
        var passes = new List<EffectPass>();

        foreach (var pass in document.Passes)
        {
            var analysis = EffectAnalyzer.Analyze(pass);

            var compiled = type switch
            {
                EffectType.Compute => compileComputeShader(pass, analysis),
                EffectType.Graphics => compileGraphicsShader(pass, analysis),
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };

            var elements = new List<LayoutElementDescription>();

            foreach (var parameter in analysis.Parameters)
            {
                var elementDescriptor = new LayoutElementDescription
                {
                    Name = parameter.Name,
                    Flags = LayoutElementFlags.None,
                    Stages = type == EffectType.Graphics
                        ? ShaderStage.Vertex | ShaderStage.Fragment
                        : ShaderStage.Compute,
                };

                if (parameter.Flags.HasFlag(EffectParameterFlags.Uniform))
                {
                    if (parameter.Flags.HasFlag(EffectParameterFlags.Image))
                    {
                        elementDescriptor.Kind = ResourceKind.TextureReadWrite;
                    }
                    else if (parameter.Flags.HasFlag(EffectParameterFlags.Texture))
                    {
                        elementDescriptor.Kind = ResourceKind.TextureReadOnly;
                    }
                    else if (parameter.Flags.HasFlag(EffectParameterFlags.Sampler))
                    {
                        elementDescriptor.Kind = ResourceKind.Sampler;
                    }
                    else
                    {
                        elementDescriptor.Kind = ResourceKind.UniformBuffer;
                    }
                }
                else if (parameter.Flags.HasFlag(EffectParameterFlags.Buffer))
                {
                    elementDescriptor.Kind = ResourceKind.StructuredBufferReadWrite;
                }

                elements.Add(elementDescriptor);
            }

            var layoutDescriptor = new LayoutDescription
            {
                Elements = elements
            };

            var layout = device.Factory.CreateResourceLayout(ref layoutDescriptor);

            passes.Add(new EffectPass(pass.Name, analysis.Stages, analysis.Parameters, layout, compiled));
        }

        return new Effect(document.Name, type, device, passes);
    }

    private IShader[] compileGraphicsShader(EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        string vertCode = vertTranspiler.Transpile(pass, analysis);
        string fragCode = fragTranspiler.Transpile(pass, analysis);

        var vert = device.CompileShader(vertCode, ShaderStage.Vertex, new ShaderCompilationOptions(pass.Name));
        var frag = device.CompileShader(fragCode, ShaderStage.Fragment, new ShaderCompilationOptions(pass.Name));

        var shaderDescriptor = new ShaderDescription
        {
            Code = vert.Code,
            Stage = ShaderStage.Vertex,
            EntryPoint = device.GraphicsAPI != GraphicsAPI.Metal ? "main" : "main0",
        };

        var vertShader = device.Factory.CreateShader(ref shaderDescriptor);

        shaderDescriptor.Code = frag.Code;
        shaderDescriptor.Stage = ShaderStage.Fragment;

        var fragShader = device.Factory.CreateShader(ref shaderDescriptor);

        return new[] { vertShader, fragShader };
    }

    private IShader[] compileComputeShader(EffectDocumentPass pass, EffectAnalysisResult analysis)
    {
        string compCode = compTranspiler.Transpile(pass, analysis);
        var comp = device.CompileShader(compCode, ShaderStage.Compute, new ShaderCompilationOptions(pass.Name));

        var shaderDescriptor = new ShaderDescription
        {
            Code = comp.Code,
            Stage = ShaderStage.Compute,
            EntryPoint = device.GraphicsAPI != GraphicsAPI.Metal ? "main" : "main0",
        };

        return new[] { device.Factory.CreateShader(ref shaderDescriptor) };
    }
}
