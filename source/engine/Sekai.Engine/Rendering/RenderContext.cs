// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Engine.Extensions;
using Sekai.Engine.Graphics;
using Sekai.Framework.Annotations;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

public class RenderContext : SceneSystem, IRenderable
{
    [Resolved]
    private IGraphicsDevice device { get; set; } = null!;

    /// <summary>
    /// The world matrix for this render context.
    /// </summary>
    public Matrix4x4 WorldMatrix = Matrix4x4.Identity;

    private ICommandQueue queue = null!;
    private GraphicsPipelineDescription pipelineDescriptor;
    private MutablePipeline mutablePipeline = null!;
    private readonly List<MeshComponent> meshes = new();
    private readonly Dictionary<string, IBindableResource> parameters = new();
    internal IReadOnlyDictionary<string, IBindableResource> Parameters => parameters;
    internal override bool IsAlive => Scene?.Camera != null && queue != null && base.IsAlive;

    protected override void Load()
    {
        queue = device.Factory.CreateCommandQueue();
        mutablePipeline = new MutablePipeline(device);

        pipelineDescriptor = new GraphicsPipelineDescription
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
            Outputs = device.SwapChain.Framebuffer.OutputDescription,
        };

        AddParameter<Matrix4x4>("View");
        AddParameter<Matrix4x4>("Projection");
    }

    public void Render()
    {
        queue.Begin();
        queue.SetFramebuffer(device.SwapChain.Framebuffer);

        UpdateParameter("View", Scene.Camera.ViewMatrix);
        UpdateParameter("Projection", Scene.Camera.ProjMatrix);

        for (int i = 0; i < meshes.Count; i++)
        {
            var component = meshes[i];
            var mesh = meshes[i].Mesh;

            foreach (var effectPass in mesh.Material.Effect.Passes)
            {
                var pass = mesh.Material[effectPass.Name];

                var transform = meshes[i].Entity.GetComponent<Transform>()!;
                pass.SetUniformValue("m_Model", transform.WorldMatrix);

                pipelineDescriptor.Topology = mesh.Topology;
                pipelineDescriptor.Layouts[0] = pass.Pass.Layout;
                pipelineDescriptor.ShaderSet.Shaders = pass.Pass.Shaders;
                pipelineDescriptor.Rasterizer.Culling = mesh.Culling;
                pipelineDescriptor.Rasterizer.Winding = mesh.Winding;
                pipelineDescriptor.Rasterizer.FillMode = pass.FillMode;
                pipelineDescriptor.Blend.Attachments[0] = pass.Blending;
                pipelineDescriptor.ShaderSet.Layouts[0] = mesh.VertexBuffer.Layout;

                queue.SetIndexBuffer(mesh.IndexBuffer);
                queue.SetVertexBuffer(mesh.VertexBuffer);
                queue.SetPipeline(mutablePipeline.GetPipeline(pipelineDescriptor));
                queue.SetResourceSet(0, pass.Resources);
                queue.DrawIndexed((uint)mesh.IndexBuffer.Count, 1, 0, 0, 0);
            }
        }

        queue.End();
        device.Submit(queue);
    }

    public void Add(MeshComponent mesh)
    {
        if (!mesh.Entity?.HasComponent<Transform>() ?? false)
            throw new InvalidOperationException(@"The entity owning the mesh component must have a transform.");

        if (meshes.Contains(mesh))
            return;

        mesh.Mesh.Material["Default"].Initialize(this);
        meshes.Add(mesh);
    }

    public void Remove(MeshComponent mesh)
    {
        meshes.Remove(mesh);
    }

    /// <summary>
    /// Adds a global parameter to be used by materials.
    /// </summary>
    protected void AddParameter<T>(string name, bool asUniform = true)
        where T : struct, IEquatable<T>
    {
        if (!IsLoaded)
            throw new InvalidOperationException();

        name = $"g_{name}";

        if (parameters.ContainsKey(name))
            throw new InvalidOperationException();

        int size = Marshal.SizeOf<T>();
        var desc = new BufferDescription((uint)size, asUniform ? BufferUsage.Uniform | BufferUsage.Dynamic : BufferUsage.StructuredReadOnly);
        parameters.Add(name, device.Factory.CreateBuffer(ref desc));
    }

    protected void UpdateParameter<T>(string name, T value)
        where T : struct, IEquatable<T>
    {
        if (!IsLoaded)
            throw new InvalidOperationException();

        name = $"g_{name}";

        if (parameters.TryGetValue(name, out var resource))
        {
            if (resource is not IBuffer buffer)
                throw new InvalidOperationException();

            device.UpdateBufferData(buffer, ref value, 0);
        }
    }

    /// <summary>
    /// Adds a global parameter to be used by materials.
    /// </summary>
    protected void AddParameter(string name, ITexture texture)
    {
        if (!IsLoaded)
            throw new InvalidOperationException();

        name = $"g_{name}";

        if (parameters.ContainsKey(name))
            throw new InvalidOperationException();

        parameters.Add(name, texture);
    }

    /// <summary>
    /// Adds a global parameter to be used by materials.
    /// </summary>
    protected void AddParameter(string name, ISampler sampler)
    {
        if (!IsLoaded)
            throw new InvalidOperationException();

        name = $"g_{name}";

        if (parameters.ContainsKey(name))
            throw new InvalidOperationException();

        parameters.Add(name, sampler);
    }
}
