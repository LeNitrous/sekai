// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Engine.Effects;
using Sekai.Engine.Effects.Compiler;
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
    /// Gets the effect pass of this material pass.
    /// </summary>
    public EffectPass Pass { get; }

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
    /// Gets or sets how polygons will be filled for this material pass.
    /// </summary>
    public PolygonFillMode FillMode { get; set; } = PolygonFillMode.Solid;

    /// <summary>
    /// Gets or sets the blending for this material for this material pass.
    /// </summary>
    public BlendAttachmentDescription Blending { get; set; }

    private bool hasChanged = true;
    private IResourceSet resourceSet = null!;
    private readonly IGraphicsDevice device;
    private readonly IBindableResource[] resources;
    private readonly List<string> globalResourceNames = new();

    internal MaterialPass(RenderContext context, Effect effect, EffectPass pass)
    {
        Effect = effect;
        Pass = pass;

        device = effect.Device;
        resources = new IBindableResource[pass.Parameters.Count];

        for (int i = 0; i < Pass.Parameters.Count; i++)
        {
            var param = Pass.Parameters[i];

            if (context.GetParameter(param.Name, out var resource))
            {
                globalResourceNames.Add(param.Name);
                resources[i] = resource;
                continue;
            }

            if (param.Flags.HasFlag(EffectParameterFlags.Buffer))
            {
                var usage = BufferUsage.StructuredReadWrite;
                var bufferDescriptor = new BufferDescription((uint)param.Size, usage);
                var buffer = device.Factory.CreateBuffer(ref bufferDescriptor);
                resources[i] = buffer;
                continue;
            }

            if (param.Flags.HasFlag(EffectParameterFlags.Uniform) && !param.Flags.HasFlag(EffectParameterFlags.Image) && !param.Flags.HasFlag(EffectParameterFlags.Texture) && !param.Flags.HasFlag(EffectParameterFlags.Sampler))
            {
                var bufferDescriptor = new BufferDescription((uint)param.Size, BufferUsage.Uniform | BufferUsage.Dynamic);
                var buffer = device.Factory.CreateBuffer(ref bufferDescriptor);
                resources[i] = buffer;
                continue;
            }
        }
    }

    public void SetTexture(string name, ITexture texture)
    {
        setResource(name, texture.Usage.HasFlag(TextureUsage.Storage) ? EffectParameterFlags.Image : EffectParameterFlags.Texture, texture, isValidTexture);
    }

    public void SetSampler(string name, ISampler sampler)
    {
        setResource(name, EffectParameterFlags.Sampler, sampler);
    }

    public void SetBufferValue<T>(string name, T value)
        where T : struct
    {
        SetBufferValue(name, ref value);
    }

    public void SetBufferValue<T>(string name, ref T value)
        where T : struct
    {
        updateBuffer(name, EffectParameterFlags.Buffer, ref value);
    }

    public void SetUniformValue<T>(string name, T value)
        where T : struct
    {
        SetUniformValue(name, ref value);
    }

    public void SetUniformValue<T>(string name, ref T value)
        where T : struct
    {
        updateBuffer(name, EffectParameterFlags.Uniform, ref value);
    }

    private void setResource(string name, EffectParameterFlags flag, IBindableResource resource, Func<IBindableResource, EffectParameterInfo, bool>? checkValid = null)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Material));

        if (globalResourceNames.Contains(name))
            throw new InvalidOperationException(@"Cannot modify a global resource.");

        bool success = false;

        for (int i = 0; i < Pass.Parameters.Count; i++)
        {
            var param = Pass.Parameters[i];

            if (param.Name != name && !param.Flags.HasFlag(flag))
                continue;

            if (!checkValid?.Invoke(resource, param) ?? false)
                throw new InvalidOperationException();

            success = true;
            resources[i] = resource;
        }

        if (!success)
            throw new InvalidOperationException($"There is no resource named \"{name}\".");

        hasChanged = true;
    }

    private unsafe void updateBuffer<T>(string name, EffectParameterFlags flag, ref T value)
        where T : struct
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Material));

        if (globalResourceNames.Contains(name))
            throw new InvalidOperationException(@"Cannot modify a global resource.");

        bool success = false;

        for (int i = 0; i < Pass.Parameters.Count; i++)
        {
            var parameter = Pass.Parameters[i];

            if (parameter.Name != name || !parameter.Flags.HasFlag(flag))
                continue;

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

        for (int i = 0; i < Pass.Parameters.Count; i++)
        {
            var parameter = Pass.Parameters[i];

            if (parameter.Flags.HasFlag(EffectParameterFlags.Texture) && resources[i] == null)
                resources[i] = device.WhitePixel;

            if (parameter.Flags.HasFlag(EffectParameterFlags.Sampler) && resources[i] == null)
                resources[i] = device.SamplerPoint;

            if (parameter.Flags.HasFlag(EffectParameterFlags.Image) && resources[i] == null)
                throw new InvalidOperationException($"Parameter \"{parameter.Name}\" must be populated with a texture that has a {nameof(TextureUsage.Storage)} flag.");
        }

        var descriptor = new ResourceSetDescription(Pass.Layout, resources);
        resourceSet = device.Factory.CreateResourceSet(ref descriptor);

        hasChanged = false;
    }

    private static bool isValidTexture(IBindableResource resource, EffectParameterInfo param)
    {
        var texture = (ITexture)resource!;

        if (texture.Usage.HasFlag(TextureUsage.Cubemap) && param.Flags.HasFlag(EffectParameterFlags.Cubemap))
            return true;

        if (texture.Usage.HasFlag(TextureUsage.Sampled) && param.Flags.HasFlag(EffectParameterFlags.Texture))
            return true;

        if (texture.Usage.HasFlag(TextureUsage.Storage) && param.Flags.HasFlag(EffectParameterFlags.Image))
            return true;

        if (texture.Kind == TextureKind.Texture1D && param.Flags.HasFlag(EffectParameterFlags.Texture1D))
            return true;

        if (texture.Kind == TextureKind.Texture2D && param.Flags.HasFlag(EffectParameterFlags.Texture2D))
            return true;

        if (texture.Kind == TextureKind.Texture3D && param.Flags.HasFlag(EffectParameterFlags.Texture3D))
            return true;

        return false;
    }

    protected override void Destroy()
    {
        resourceSet?.Dispose();

        for (int i = 0; i < Pass.Parameters.Count; i++)
        {
            var param = Pass.Parameters[i];

            if (globalResourceNames.Contains(param.Name))
                continue;

            if (resources[i] is IBuffer buffer)
                buffer.Dispose();

            resources[i] = null!;
        }
    }
}
