// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using System.Text;
using Sekai.Framework.Graphics;
using Veldrid.SPIRV;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal partial class VeldridGraphicsDevice
{
    public ShaderCompilationResult CompileShader(string source, ShaderStage stage, ShaderCompilationOptions? options = null)
    {
        if (stage is ShaderStage.Fragment or ShaderStage.Vertex or ShaderStage.Compute)
        {
            options ??= new ShaderCompilationOptions();

            byte[] code = null!;
            byte[] bytes = null!;
            Exception exception = null!;
            ShaderReflectionResult reflection = default;

            try
            {
                var res = SpirvCompilation.CompileGlslToSpirv(source, options.Filename, stage.ToVeldrid(), new GlslCompileOptions(true, options.Macros.Select(m => new MacroDefinition(m.Key, m.Value)).ToArray()));
                bytes = res.SpirvBytes;

                var target = getCompileTarget(GraphicsAPI);

                if (stage is ShaderStage.Fragment)
                {
                    var frag = SpirvCompilation.CompileVertexFragment(emptyShader, res.SpirvBytes, target);
                    reflection = fromVeldrid(frag.Reflection);
                    code = GraphicsAPI == GraphicsAPI.Vulkan ? res.SpirvBytes : getShaderBytes(GraphicsAPI, frag.FragmentShader);
                }

                if (stage is ShaderStage.Vertex)
                {
                    var vert = SpirvCompilation.CompileVertexFragment(res.SpirvBytes, emptyShader, target);
                    reflection = fromVeldrid(vert.Reflection);
                    code = GraphicsAPI == GraphicsAPI.Vulkan ? res.SpirvBytes : getShaderBytes(GraphicsAPI, vert.VertexShader);
                }

                if (stage is ShaderStage.Compute)
                {
                    var comp = SpirvCompilation.CompileCompute(res.SpirvBytes, target);
                    reflection = fromVeldrid(comp.Reflection);
                    code = GraphicsAPI == GraphicsAPI.Vulkan ? res.SpirvBytes : getShaderBytes(GraphicsAPI, comp.ComputeShader);
                }
            }
            catch (SpirvCompilationException scex)
            {
                string message = scex.Message.Replace("Compilation failed: ", string.Empty);
                exception = new ShaderCompilationException(message);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
                throw exception;

            return new ShaderCompilationResult(options.Filename, code, bytes, stage, reflection);
        }

        throw new NotSupportedException($"Compiling for {stage} is not supported.");
    }

    private static CrossCompileTarget getCompileTarget(GraphicsAPI api)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 => CrossCompileTarget.HLSL,
            GraphicsAPI.OpenGLES => CrossCompileTarget.ESSL,
            GraphicsAPI.OpenGL => CrossCompileTarget.GLSL,
            GraphicsAPI.Metal => CrossCompileTarget.MSL,
            _ => CrossCompileTarget.GLSL,
        };
    }

    private static byte[] getShaderBytes(GraphicsAPI api, string code)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 or GraphicsAPI.OpenGLES or GraphicsAPI.OpenGL => Encoding.ASCII.GetBytes(code),
            GraphicsAPI.Metal => Encoding.UTF8.GetBytes(code),
            _ => throw new NotSupportedException($"{api} is not a supported target."),
        };
    }

    private static VertexElementDescription fromVeldrid(Vd.VertexElementDescription desc)
    {
        return new VertexElementDescription
        {
            Name = desc.Name,
            Format = (VertexElementFormat)desc.Format,
            Offset = desc.Offset,
        };
    }

    private static LayoutElementDescription fromVeldrid(Vd.ResourceLayoutElementDescription desc)
    {
        return new LayoutElementDescription
        {
            Name = desc.Name,
            Kind = (ResourceKind)desc.Kind,
            Flags = (LayoutElementFlags)desc.Options,
            Stages = (ShaderStage)desc.Stages,
        };
    }

    private static LayoutDescription fromVeldrid(Vd.ResourceLayoutDescription desc)
    {
        return new LayoutDescription(desc.Elements.Select(fromVeldrid).ToArray());
    }

    private static ShaderReflectionResult fromVeldrid(SpirvReflection reflection)
    {
        return new ShaderReflectionResult(reflection.VertexElements.Select(fromVeldrid).ToArray(), reflection.ResourceLayouts.Select(fromVeldrid).ToArray());
    }

    private static readonly string emptyShaderString = @"
#version 450
void main()
{
}
";

    private static readonly byte[] emptyShader = Encoding.ASCII.GetBytes(emptyShaderString);
}
