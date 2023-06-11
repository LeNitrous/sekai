// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed class GLGraphicsAdapter : GraphicsAdapter
{
    public override string Name { get; }

    public override string Vendor { get; }

    public override Version Version { get; }

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    private GraphicsDeviceFeatures[]? featuresArray;
    private readonly GraphicsDeviceFeatures features;

    public GLGraphicsAdapter(GL gl)
    {
        GL = gl;
        GL.GetInteger(GetPName.MajorVersion, out int major);
        GL.GetInteger(GetPName.MinorVersion, out int minor);

        Name = GL.GetStringS(StringName.Renderer);
        Vendor = GL.GetStringS(StringName.Vendor);
        Version = new(major, minor);

        if (GL.IsExtensionPresent("GL_ARB_compute_shader"))
        {
            features |= GraphicsDeviceFeatures.ShaderCompute;
        }

        if (GL.IsExtensionPresent("GL_ARB_geometry_shader"))
        {
            features |= GraphicsDeviceFeatures.ShaderGeometry;
        }

        if (GL.IsExtensionPresent("GL_ARB_tesselation_shader"))
        {
            features |= GraphicsDeviceFeatures.ShaderTesselation;
        }

        if (GL.IsExtensionPresent("GL_ARB_gpu_shader_fp64"))
        {
            features |= GraphicsDeviceFeatures.ShaderFloat64;
        }

        features |= GraphicsDeviceFeatures.SamplerLODBias;

        if (GL.IsExtensionPresent("GL_EXT_texture_filter_anisotropic") || GL.IsExtensionPresent("GL_ARB_texture_filter_anisotropic"))
        {
            features |= GraphicsDeviceFeatures.SamplerAnisotropy;
        }

        if (Version >= new Version(3, 2) || GL.IsExtensionPresent("GL_ARB_draw_elements_base_vertex") || GL.IsExtensionPresent("GL_OES_draw_elements_base_vertex"))
        {
            features |= GraphicsDeviceFeatures.DrawBaseVertex;
        }

        if (Version >= new Version(4, 2))
        {
            features |= GraphicsDeviceFeatures.DrawBaseInstance;
        }

        if (Version >= new Version(4, 0) || GL.IsExtensionPresent("GL_ARB_draw_indirect") || GL.IsExtensionPresent("GL_ARB_multi_draw_indirect") || GL.IsExtensionPresent("GL_EXT_multi_draw_indirect"))
        {
            features |= GraphicsDeviceFeatures.DrawIndirect;
            features |= GraphicsDeviceFeatures.DrawIndirectBaseInstance;
        }

        features |= GraphicsDeviceFeatures.FillModeWireframe;
        features |= GraphicsDeviceFeatures.Texture1D;

        if (Version >= new Version(4, 3) || GL.IsExtensionPresent("GL_ARB_texture_view") || GL.IsExtensionPresent("GL_OES_texture_view"))
        {
            features |= GraphicsDeviceFeatures.TextureSubsetView;
        }

        if (Version >= new Version(4, 5) || GL.IsExtensionPresent("GL_ARB_clip_control"))
        {
            GL.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.ZeroToOne);
            features |= GraphicsDeviceFeatures.DepthRangeZeroToOne;
        }

        if (Version >= new Version(4, 0))
        {
            features |= GraphicsDeviceFeatures.FramebufferAttachmentBlending;
        }

        if (Version >= new Version(4, 3) || GL.IsExtensionPresent("GL_ARB_shader_storage_buffer_object"))
        {
            features |= GraphicsDeviceFeatures.BufferStructured;
        }

        if (GL.IsExtensionPresent("GL_ARB_uniform_buffer_object"))
        {
            features |= GraphicsDeviceFeatures.BufferSubsetView;
        }
    }

    public override IEnumerable<GraphicsDeviceFeatures> GetDeviceFeatures()
    {
        return featuresArray ??= GraphicsDeviceFeaturesExtensions.GetValues();
    }

    public override bool IsFeatureSupported(GraphicsDeviceFeatures feature)
    {
        return features.HasFlagFast(feature);
    }
}
