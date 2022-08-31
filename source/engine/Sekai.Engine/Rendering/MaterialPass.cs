// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sekai.Engine.Effects;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

public class MaterialPass : FrameworkObject
{
    /// <summary>
    /// Gets the effect used by this material pass.
    /// </summary>
    public Effect Effect { get; }

    /// <summary>
    /// Gets the resources used by this material pass.
    /// </summary>
    public IResourceSet Resources
    {
        get
        {
            if (hasChanged)
                invalidateResourceSet();

            return resourceSet;
        }
    }

    /// <summary>
    /// Gets the resource layout for this material pass.
    /// </summary>
    public IResourceLayout Layout { get; }

    /// <summary>
    /// Gets or sets how polygons will be filled for this material pass.
    /// </summary>
    public PolygonFillMode FillMode { get; set; } = PolygonFillMode.Solid;

    /// <summary>
    /// Gets or sets the blending for this material for this material pass.
    /// </summary>
    public BlendAttachmentDescription Blending { get; set; }

    private bool hasChanged = true;
    private bool initialized = false;
    private IResourceSet resourceSet = null!;
    private readonly IGraphicsDevice device;
    private readonly IBindableResource[] resources;
    private readonly List<string> globalResourceNames = new();

    internal MaterialPass(Effect effect)
    {
        Effect = effect;

        device = effect.Device;
        resources = new IBindableResource[effect.Resources.Count];

        var elements = new List<LayoutElementDescription>();

        for (int i = 0; i < effect.Resources.Count; i++)
        {
            var member = effect.Resources[i];
            var element = new LayoutElementDescription
            {
                Name = member.Name,
                Flags = LayoutElementFlags.None,
                Stages = ShaderStage.Vertex | ShaderStage.Fragment
            };

            if (member.Flags.HasFlag(EffectMemberFlags.Buffer))
            {
                element.Kind = member.Flags.HasFlag(EffectMemberFlags.ReadWrite) ? ResourceKind.StructuredBufferReadWrite : ResourceKind.StructuredBufferReadOnly;
            }

            if (member.Flags.HasFlag(EffectMemberFlags.Uniform))
            {
                element.Kind = ResourceKind.UniformBuffer;
            }

            if (member.Flags.HasFlag(EffectMemberFlags.Texture))
            {
                element.Kind = member.Flags.HasFlag(EffectMemberFlags.ReadWrite) ? ResourceKind.TextureReadWrite : ResourceKind.TextureReadOnly;
            }

            if (member.Flags.HasFlag(EffectMemberFlags.Sampler))
            {
                element.Kind = ResourceKind.Sampler;
            }

            elements.Add(element);
        }

        var layoutDescriptor = new LayoutDescription(elements);
        Layout = device.Factory.CreateResourceLayout(ref layoutDescriptor);
    }

    internal void Initialize(RenderContext context)
    {
        if (initialized)
            return;

        for (int i = 0; i < Effect.Resources.Count; i++)
        {
            var member = Effect.Resources[i];

            if (context.Parameters.TryGetValue(member.Name, out var resource))
            {
                globalResourceNames.Add(member.Name);
                resources[i] = resource;
                continue;
            }

            if (member.Flags.HasFlag(EffectMemberFlags.Buffer))
            {
                var usage = member.Flags.HasFlag(EffectMemberFlags.ReadWrite) ? BufferUsage.StructuredReadWrite : BufferUsage.StructuredReadOnly;
                var bufferDescriptor = new BufferDescription((uint)member.Size, usage);
                var buffer = device.Factory.CreateBuffer(ref bufferDescriptor);
                resources[i] = buffer;
                continue;
            }

            if (member.Flags.HasFlag(EffectMemberFlags.Uniform))
            {
                var bufferDescriptor = new BufferDescription((uint)member.Size, BufferUsage.Uniform | BufferUsage.Dynamic);
                var buffer = device.Factory.CreateBuffer(ref bufferDescriptor);
                resources[i] = buffer;
                continue;
            }
        }

        initialized = true;
    }

    public void SetTexture(string name, ITexture texture)
    {
        setResource(name, EffectMemberFlags.Texture, texture, isValidTexture);
    }

    public void SetSampler(string name, ISampler sampler)
    {
        setResource(name, EffectMemberFlags.Sampler, sampler, (_, _) => true);
    }

    public void SetBufferValue<T>(string name, T value)
        where T : struct
    {
        SetBufferValue(name, ref value);
    }

    public void SetBufferValue<T>(string name, ref T value)
        where T : struct
    {
        updateBuffer(name, EffectMemberFlags.Buffer, ref value);
    }

    public void SetUniformValue<T>(string name, T value)
        where T : struct
    {
        SetUniformValue(name, ref value);
    }

    public void SetUniformValue<T>(string name, ref T value)
        where T : struct
    {
        updateBuffer(name, EffectMemberFlags.Uniform, ref value);
    }

    private void setResource(string name, EffectMemberFlags flag, IBindableResource resource, Func<IBindableResource, EffectMember, bool> checkValid)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Material));

        if (globalResourceNames.Contains(name))
            throw new InvalidOperationException(@"Cannot modify a global resource.");

        bool success = false;

        for (int i = 0; i < Effect.Resources.Count; i++)
        {
            var member = Effect.Resources[i];

            if (member.Name != name && !member.Flags.HasFlag(flag))
                continue;

            if (!checkValid(resource, member))
                throw new InvalidOperationException();

            success = true;
            resources[i] = resource;
        }

        if (!success)
            throw new InvalidOperationException($"There is no resource named \"{name}\".");
    }

    private unsafe void updateBuffer<T>(string name, EffectMemberFlags flag, ref T value)
        where T : struct
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Material));

        if (globalResourceNames.Contains(name))
            throw new InvalidOperationException(@"Cannot modify a global resource.");

        bool success = false;

        for (int i = 0; i < Effect.Resources.Count; i++)
        {
            var member = Effect.Resources[i];

            if (member.Name != name || !member.Flags.HasFlag(flag))
                continue;

            if (member is not EffectMember<T> || member.Size != Marshal.SizeOf<T>())
                throw new InvalidOperationException();

            var resource = resources[i];

            if (resource is not IBuffer buffer)
                throw new InvalidOperationException(@"Resource is not a buffer.");

            success = true;
            device.UpdateBufferData(buffer, ref value, 0);
        }

        if (!success)
            throw new InvalidOperationException($"There is no resource named \"{name}\".");
    }

    private void invalidateResourceSet()
    {
        resourceSet?.Dispose();
        resourceSet = null!;

        // TODO: replace empty texture slots with white texture
        // TODO: replace empty sampler slots with point sampler
        var descriptor = new ResourceSetDescription(Layout, resources);
        resourceSet = device.Factory.CreateResourceSet(ref descriptor);

        hasChanged = false;
    }

    private static bool isValidTexture(IBindableResource resource, EffectMember member)
    {
        var texture = (ITexture)resource!;

        if (texture.Usage.HasFlag(TextureUsage.Cubemap) && member.Flags.HasFlag(EffectMemberFlags.Cubemap))
            return true;

        if (texture.Usage.HasFlag(TextureUsage.Sampled) && !member.Flags.HasFlag(EffectMemberFlags.ReadWrite))
            return true;

        if (texture.Usage.HasFlag(TextureUsage.Storage) && member.Flags.HasFlag(EffectMemberFlags.ReadWrite))
            return true;

        return false;
    }

    protected override void Destroy()
    {
        resourceSet?.Dispose();
        Layout.Dispose();

        for (int i = 0; i < Effect.Resources.Count; i++)
        {
            var member = Effect.Resources[i];

            if (globalResourceNames.Contains(member.Name))
                continue;

            if (resources[i] is IBuffer buffer)
                buffer.Dispose();

            resources[i] = null!;
        }
    }
}
