// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Text;
using Sekai.Engine.Effects;
using Sekai.Engine.Effects.Analysis;
using Sekai.Engine.Effects.Documents;
using Sekai.Engine.Effects.Transpiler;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Assets;

public sealed class EffectLoader : IAssetLoader<Effect>
{
    private readonly IGraphicsDevice device = Game.Resolve<IGraphicsDevice>();
    private readonly EffectVertexTranspiler vertTranspiler = new();
    private readonly EffectFragmentTranspiler fragTranspiler = new();

    public Effect Load(ReadOnlySpan<byte> data)
    {
        string source = Encoding.UTF8.GetString(data);

        var document = EffectDocument.Load(source);
        var passes = new List<EffectPass>();

        foreach (var pass in document.Passes)
        {
            var analysis = EffectAnalyzer.Analyze(pass);
            var compiled = compileGraphicsShader(pass, analysis);
            var elements = new List<LayoutElementDescription>();

            foreach (var parameter in analysis.Parameters)
            {
                var elementDescriptor = new LayoutElementDescription
                {
                    Name = parameter.Name,
                    Flags = LayoutElementFlags.None,
                    Stages = ShaderStage.Vertex | ShaderStage.Fragment,
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

        return new Effect(document.Name, EffectType.Graphics, device, passes);
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
}
