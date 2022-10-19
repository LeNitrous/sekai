// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Extensions;
using Sekai.Engine.Graphics;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

public sealed class MeshRenderPipeline : RenderFeature<MeshRenderObject>
{
    private GraphicsPipelineDescription descriptor;
    private readonly MutablePipeline pipeline = new();

    public MeshRenderPipeline()
    {
        descriptor = new GraphicsPipelineDescription
        {
            DepthStencil = new DepthStencilStateDescription
            {
                DepthTest = true,
                DepthWrite = true,
                DepthComparison = ComparisonKind.LessThanOrEqual,
            },
            Rasterizer = new RasterizerStateDescription
            {
                DepthClip = true,
                ScissorTest = false,
            },
            ShaderSet = new ShaderSetDescription
            {
                Layouts = new VertexLayoutDescription[1],
                Constants = Array.Empty<ShaderConstant>(),
            },
            Blend = new BlendStateDescription
            {
                Attachments = new BlendAttachmentDescription[1],
            },
            Layouts = new IResourceLayout[1],
            Outputs = Device.SwapChain.Framebuffer.OutputDescription,
        };
    }

    public override void Render(RenderContext context, string stage, ICommandQueue commands, MeshRenderObject renderable)
    {
        var mesh = renderable.Mesh;
        var pass = mesh.Material[stage];

        pass.SetUniformValue("m_Model", renderable.WorldMatrix);
        descriptor.Topology = mesh.Topology;
        descriptor.Layouts[0] = pass.Pass.Layout;
        descriptor.ShaderSet.Shaders = pass.Pass.Shaders;
        descriptor.Rasterizer.Culling = mesh.Culling;
        descriptor.Rasterizer.Winding = mesh.Winding;
        descriptor.Rasterizer.FillMode = pass.FillMode;
        descriptor.Blend.Attachments[0] = pass.Blending;
        descriptor.ShaderSet.Layouts[0] = mesh.VertexBuffer.Layout;

        commands.SetIndexBuffer(mesh.IndexBuffer);
        commands.SetVertexBuffer(mesh.VertexBuffer);
        commands.SetPipeline(pipeline.GetPipeline(descriptor));
        commands.SetResourceSet(0, pass.Resources);
        commands.DrawIndexed((uint)mesh.IndexBuffer.Count, 1, 0, 0, 0);
    }
}
