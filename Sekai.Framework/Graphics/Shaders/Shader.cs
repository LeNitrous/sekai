// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sekai.Framework.Extensions;
using Sekai.Framework.Platform;
using Veldrid.SPIRV;

namespace Sekai.Framework.Graphics.Shaders;

public class Shader : GraphicsObjectCollection<Veldrid.Shader>, IEquatable<Shader>
{
    internal Veldrid.ResourceSet[]? ResourceSets { get; private set; }
    internal override IReadOnlyList<Veldrid.Shader> Resources { get; }
    internal readonly Veldrid.ResourceLayout[] ResourceLayouts;
    internal readonly Veldrid.VertexLayoutDescription VertexLayout;
    private IBindableResource[][] bindings = Array.Empty<IBindableResource[]>();

    /// <summary>
    /// Shader parts that is used to create this shader program.
    /// </summary>
    public readonly IReadOnlyList<ShaderPart> Parts;

    /// <summary>
    /// Gets or sets the bindable resources this shader will use during execution.
    /// </summary>
    public IBindableResource[][] Bindings
    {
        get => bindings;
        set
        {
            bindings = value;

            if (ResourceSets != null)
            {
                foreach (var resourceSet in ResourceSets)
                    resourceSet.Dispose();
            }

            ResourceSets = new Veldrid.ResourceSet[bindings.Length];

            for (int i = 0; i < bindings.Length; i++)
                ResourceSets[i] = Context.Resources.CreateResourceSet(new Veldrid.ResourceSetDescription(ResourceLayouts[i], bindings[i].Select(b => b.Resource).ToArray()));
        }
    }

    public Shader(params ShaderPart[] parts)
    {
        Parts = parts;

        if (Parts.Count == 1)
        {
            var part = Parts.SingleOrDefault();

            if (part == null)
                throw new InvalidOperationException(@"There must be a compute shader when only one shader part is passed.");

            byte[] byteCode;

            var result = SpirvCompilation.CompileCompute(part.ByteCode, getCompileTarget(Context.API));
            VertexLayout = new(result.Reflection.VertexElements);
            ResourceLayouts = result.Reflection.ResourceLayouts.Select(layout => Context.FetchResourceLayout(layout)).ToArray();

            if (Context.API == GraphicsAPI.Vulkan)
            {
                byteCode = part.ByteCode;
            }
            else
            {
                byteCode = getBytes(Context.API, result.ComputeShader);
            }

            Resources = new[]
            {
                Context.Resources.CreateShader(new(part.Stage.ToVeldrid(), byteCode, Context.API == GraphicsAPI.Metal ? "main0" : "main")),
            };
        }

        if (Parts.Count == 2)
        {
            var vertPart = Parts.SingleOrDefault(s => s.Stage == ShaderStage.Vertex);
            var fragPart = Parts.SingleOrDefault(s => s.Stage == ShaderStage.Fragment);

            if (vertPart == null || fragPart == null)
                throw new InvalidOperationException(@"There must be a vertex and fragment shader pair when two shader parts are passed.");

            byte[] vertByteCode;
            byte[] fragByteCode;

            var result = SpirvCompilation.CompileVertexFragment(vertPart.ByteCode, fragPart.ByteCode, getCompileTarget(Context.API));
            VertexLayout = new(result.Reflection.VertexElements);
            ResourceLayouts = result.Reflection.ResourceLayouts.Select(layout => Context.Resources.CreateResourceLayout(layout)).ToArray();

            if (Context.API == GraphicsAPI.Vulkan)
            {
                vertByteCode = vertPart.ByteCode;
                fragByteCode = fragPart.ByteCode;
            }
            else
            {
                vertByteCode = getBytes(Context.API, result.VertexShader);
                fragByteCode = getBytes(Context.API, result.FragmentShader);
            }

            Resources = new[]
            {
                Context.Resources.CreateShader(new(vertPart.Stage.ToVeldrid(), vertByteCode, Context.API == GraphicsAPI.Metal ? "main0" : "main")),
                Context.Resources.CreateShader(new(fragPart.Stage.ToVeldrid(), fragByteCode, Context.API == GraphicsAPI.Metal ? "main0" : "main")),
            };
        }

        if (Resources == null || ResourceLayouts == null)
            throw new NotSupportedException();
    }

    protected override void Destroy()
    {
        if (ResourceSets != null)
        {
            foreach (var resourceSet in ResourceSets)
                resourceSet.Dispose();
        }

        base.Destroy();
    }

    private static CrossCompileTarget getCompileTarget(GraphicsAPI api)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 => CrossCompileTarget.HLSL,
            GraphicsAPI.OpenGL when RuntimeInfo.IsDesktop => CrossCompileTarget.GLSL,
            GraphicsAPI.OpenGL when RuntimeInfo.IsMobile => CrossCompileTarget.ESSL,
            GraphicsAPI.Metal => CrossCompileTarget.MSL,
            _ => CrossCompileTarget.GLSL,
        };
    }

    private static byte[] getBytes(GraphicsAPI api, string code)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 or GraphicsAPI.OpenGL => Encoding.ASCII.GetBytes(code),
            GraphicsAPI.Metal => Encoding.UTF8.GetBytes(code),
            _ => throw new NotSupportedException(),
        };
    }

    public bool Equals(Shader? other)
    {
        return other is not null
            && Parts.SequenceEqual(other.Parts, EqualityComparer<ShaderPart>.Default);
    }

    public override bool Equals(object? obj)
        => Equals(obj as Shader);

    public override int GetHashCode()
        => HashCode.Combine(IsDisposed, Context, Resources, Parts, Resources, ResourceLayouts, VertexLayout);
}
