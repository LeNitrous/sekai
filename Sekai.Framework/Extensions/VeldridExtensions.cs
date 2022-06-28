// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Graphics;
using Sekai.Framework.Graphics.Buffers;
using Sekai.Framework.Graphics.Shaders;
using Sekai.Framework.Graphics.Textures;
using Sekai.Framework.Graphics.Vertices;
using Sekai.Framework.Platform;

namespace Sekai.Framework.Extensions;

internal static class VeldridExtensions
{
    public static Veldrid.BufferUsage ToVeldrid(this BufferUsage usage)
    {
        return (Veldrid.BufferUsage)usage;
    }

    public static Veldrid.MapMode ToVeldrid(this MapMode mode)
    {
        if (mode.HasFlag(MapMode.Read) && mode.HasFlag(MapMode.Write))
            return Veldrid.MapMode.ReadWrite;

        if (mode == MapMode.Read)
            return Veldrid.MapMode.Read;

        if (mode == MapMode.Write)
            return Veldrid.MapMode.Write;

        throw new ArgumentOutOfRangeException(nameof(mode));
    }

    public static Veldrid.TextureSampleCount ToVeldrid(this TextureSampleCount sampleCount)
    {
        return (Veldrid.TextureSampleCount)sampleCount;
    }

    public static Veldrid.TextureUsage ToVeldrid(this TextureUsage usage)
    {
        return (Veldrid.TextureUsage)usage;
    }

    public static Veldrid.TextureType ToVeldrid(this TextureKind kind)
    {
        return (Veldrid.TextureType)kind;
    }

    public static Veldrid.PixelFormat ToVeldrid(this PixelFormat format)
    {
        return (Veldrid.PixelFormat)format;
    }

    public static Veldrid.SamplerBorderColor ToVeldrid(this SamplerBorder border)
    {
        return (Veldrid.SamplerBorderColor)border;
    }

    public static Veldrid.SamplerFilter ToVeldrid(this SamplerFilter filter)
    {
        return (Veldrid.SamplerFilter)filter;
    }

    public static Veldrid.SamplerAddressMode ToVeldrid(this SamplerAddressMode mode)
    {
        return (Veldrid.SamplerAddressMode)mode;
    }

    public static Veldrid.ComparisonKind ToVeldrid(this ComparisonKind kind)
    {
        return (Veldrid.ComparisonKind)kind;
    }

    public static Veldrid.ShaderStages ToVeldrid(this ShaderStage stage)
    {
        return (Veldrid.ShaderStages)stage;
    }

    public static Veldrid.VertexElementFormat ToVeldrid(this VertexMemberFormat format)
    {
        return (Veldrid.VertexElementFormat)format;
    }

    public static Veldrid.PrimitiveTopology ToVeldrid(this PrimitiveTopology topology)
    {
        return (Veldrid.PrimitiveTopology)topology;
    }

    public static Veldrid.FaceCullMode ToVeldrid(this FaceCulling culling)
    {
        return (Veldrid.FaceCullMode)culling;
    }

    public static Veldrid.FrontFace ToVeldrid(this FaceWinding winding)
    {
        return (Veldrid.FrontFace)winding;
    }

    public static Veldrid.PolygonFillMode ToVeldrid(this PolygonFillMode fillMode)
    {
        return (Veldrid.PolygonFillMode)fillMode;
    }

    public static Veldrid.BlendFactor ToVeldrid(this BlendFactor factor)
    {
        return (Veldrid.BlendFactor)factor;
    }

    public static Veldrid.StencilOperation ToVeldrid(this StencilOperation operation)
    {
        return (Veldrid.StencilOperation)operation;
    }

    public static Veldrid.BlendFunction ToVeldrid(this BlendFunction function)
    {
        return (Veldrid.BlendFunction)function;
    }

    public static Veldrid.RgbaFloat ToVeldrid(this Color color)
    {
        return new Veldrid.RgbaFloat(color.R, color.G, color.B, color.A);
    }

    public static Veldrid.RasterizerStateDescription ToVeldrid(this RasterizerInfo state)
    {
        return new(state.Culling.ToVeldrid(), state.FillMode.ToVeldrid(), state.Winding.ToVeldrid(), state.DepthClip, state.ScissorTest);
    }

    public static Veldrid.BlendStateDescription ToVeldrid(this BlendInfo state)
    {
        return new(state.Factor.ToVeldrid(), state.AlphaToCoverage, state.Attachments.Select(a => a.ToVeldrid()).ToArray());
    }

    public static Veldrid.BlendAttachmentDescription ToVeldrid(this BlendAttachmentInfo state)
    {
        return new(state.Enabled, state.SourceColor.ToVeldrid(), state.DestinationColor.ToVeldrid(), state.Color.ToVeldrid(), state.SourceAlpha.ToVeldrid(), state.DestinationAlpha.ToVeldrid(), state.Alpha.ToVeldrid());
    }

    public static Veldrid.StencilBehaviorDescription ToVeldrid(this StencilBehavior behavior)
    {
        return new(behavior.Fail.ToVeldrid(), behavior.Pass.ToVeldrid(), behavior.DepthFail.ToVeldrid(), behavior.Comparison.ToVeldrid());
    }

    public static Veldrid.Viewport ToVeldrid(this Viewport viewport)
    {
        return new(viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinimumDepth, viewport.MaximumDepth);
    }

    public static Veldrid.OutputAttachmentDescription ToVeldrid(this OutputAttachmentInfo info)
    {
        return new Veldrid.OutputAttachmentDescription(info.Format.ToVeldrid());
    }

    public static Veldrid.OutputDescription ToVeldrid(this OutputInfo info)
    {
        return new Veldrid.OutputDescription(info.Depth?.ToVeldrid(), info.Color.Select(a => a.ToVeldrid()).ToArray(), info.SampleCount.ToVeldrid());
    }

    public static Veldrid.GraphicsBackend ToVeldrid(this GraphicsAPI api)
    {
        return api switch
        {
            GraphicsAPI.Direct3D11 => Veldrid.GraphicsBackend.Direct3D11,
            GraphicsAPI.Vulkan => Veldrid.GraphicsBackend.Vulkan,
            GraphicsAPI.OpenGL => RuntimeInfo.IsDesktop ? Veldrid.GraphicsBackend.OpenGL : Veldrid.GraphicsBackend.OpenGLES,
            GraphicsAPI.Metal => Veldrid.GraphicsBackend.Metal,
            _ => throw new NotSupportedException()
        };
    }
}
