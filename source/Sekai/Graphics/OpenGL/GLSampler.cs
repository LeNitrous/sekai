// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed unsafe class GLSampler : Sampler
{
    public override TextureFilter Filter { get; }
    public override TextureAddress AddressU { get; }
    public override TextureAddress AddressV { get; }
    public override TextureAddress AddressW { get; }
    public override int MaxAnisotropy { get; }
    public override Color BorderColor { get; }
    public override float LODBias { get; }
    public override float MinimumLOD { get; }
    public override float MaximumLOD { get; }

    private bool isDisposed;
    private readonly uint handle;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLSampler(GL gl, SamplerDescription description)
    {
        Filter = description.Filter;
        AddressU = description.AddressU;
        AddressV = description.AddressV;
        AddressW = description.AddressW;
        MaxAnisotropy = description.MaxAnisotropy;
        BorderColor = description.BorderColor;
        LODBias = description.LODBias;
        MinimumLOD = description.MinimumLOD;
        MaximumLOD = description.MaximumLOD;

        GL = gl;
        handle = GL.GenSampler();

        description.Filter.AsFilters(out var minFilter, out var magFilter);
        GL.SamplerParameter(handle, SamplerParameterI.MinFilter, (int)minFilter);
        GL.SamplerParameter(handle, SamplerParameterI.MagFilter, (int)magFilter);
        GL.SamplerParameter(handle, SamplerParameterI.WrapS, (int)description.AddressU.AsWrapMode());
        GL.SamplerParameter(handle, SamplerParameterI.WrapT, (int)description.AddressV.AsWrapMode());
        GL.SamplerParameter(handle, SamplerParameterI.WrapR, (int)description.AddressW.AsWrapMode());
        GL.SamplerParameter(handle, SamplerParameterI.CompareFunc, (int)DepthFunction.Lequal);

        if (description.Filter == TextureFilter.Anisotropic)
        {
            GL.SamplerParameter(handle, SamplerParameterF.MaxAnisotropy, description.MaxAnisotropy);
        }

        Span<float> borderColor = stackalloc float[]
        {
            description.BorderColor.R / 255f,
            description.BorderColor.G / 255f,
            description.BorderColor.B / 255f,
            description.BorderColor.A / 255f,
        };

        fixed (float* bColor = borderColor)
        {
            GL.SamplerParameter(handle, SamplerParameterF.BorderColor, bColor);
        }

        GL.SamplerParameter(handle, SamplerParameterF.MinLod, description.MinimumLOD);
        GL.SamplerParameter(handle, SamplerParameterF.MaxLod, description.MaximumLOD);
        GL.SamplerParameter(handle, SamplerParameterF.LodBias, description.LODBias);
    }

    public void Bind(uint slot)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLSampler));
        }

        GL.BindSampler(slot, handle);
    }

    ~GLSampler()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        GL.DeleteSampler(handle);

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
