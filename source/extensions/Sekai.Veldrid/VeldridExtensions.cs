// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal static class VeldridExtensions
{
    public static Vd.BlendFactor ToVeldrid(this BlendFactor factor)
    {
        return factor switch
        {
            BlendFactor.Zero => Vd.BlendFactor.Zero,
            BlendFactor.One => Vd.BlendFactor.One,
            BlendFactor.SourceAlpha => Vd.BlendFactor.SourceAlpha,
            BlendFactor.InverseSourceAlpha => Vd.BlendFactor.InverseSourceAlpha,
            BlendFactor.DestinationAlpha => Vd.BlendFactor.DestinationAlpha,
            BlendFactor.InverseDestinationAlpha => Vd.BlendFactor.InverseDestinationAlpha,
            BlendFactor.SourceColor => Vd.BlendFactor.SourceColor,
            BlendFactor.InverseSourceColor => Vd.BlendFactor.InverseSourceColor,
            BlendFactor.DestinationColor => Vd.BlendFactor.DestinationColor,
            BlendFactor.InverseDestinationColor => Vd.BlendFactor.InverseDestinationColor,
            BlendFactor.BlendFactor => Vd.BlendFactor.BlendFactor,
            BlendFactor.InverseBlendFactor => Vd.BlendFactor.InverseBlendFactor,
            _ => throw new ArgumentOutOfRangeException(nameof(factor)),
        };
    }

    public static Vd.BlendFunction ToVeldrid(this BlendFunction function)
    {
        return function switch
        {
            BlendFunction.Add => Vd.BlendFunction.Add,
            BlendFunction.Subtract => Vd.BlendFunction.Subtract,
            BlendFunction.ReverseSubtract => Vd.BlendFunction.ReverseSubtract,
            BlendFunction.Minimum => Vd.BlendFunction.Minimum,
            BlendFunction.Maximum => Vd.BlendFunction.Maximum,
            _ => throw new ArgumentOutOfRangeException(nameof(function)),
        };
    }

    public static Vd.BufferUsage ToVeldrid(this BufferUsage usage)
    {
        return (Vd.BufferUsage)(byte)usage;
    }

    public static Vd.RgbaFloat ToVeldrid(this Color4 color)
    {
        return new Vd.RgbaFloat(color.R, color.G, color.B, color.A);
    }

    public static Vd.ComparisonKind ToVeldrid(this ComparisonKind function)
    {
        return function switch
        {
            ComparisonKind.Never => Vd.ComparisonKind.Never,
            ComparisonKind.LessThan => Vd.ComparisonKind.Less,
            ComparisonKind.LessThanOrEqual => Vd.ComparisonKind.LessEqual,
            ComparisonKind.Equal => Vd.ComparisonKind.Equal,
            ComparisonKind.GreaterThanOrEqual => Vd.ComparisonKind.GreaterEqual,
            ComparisonKind.GreaterThan => Vd.ComparisonKind.Greater,
            ComparisonKind.NotEqual => Vd.ComparisonKind.NotEqual,
            ComparisonKind.Always => Vd.ComparisonKind.Always,
            _ => throw new ArgumentOutOfRangeException(nameof(function)),
        };
    }

    public static Vd.FaceCullMode ToVeldrid(this FaceCulling culling)
    {
        return culling switch
        {
            FaceCulling.Back => Vd.FaceCullMode.Back,
            FaceCulling.Front => Vd.FaceCullMode.Front,
            FaceCulling.None => Vd.FaceCullMode.None,
            _ => throw new ArgumentOutOfRangeException(nameof(culling)),
        };
    }

    public static Vd.FrontFace ToVeldrid(this FaceWinding winding)
    {
        return winding switch
        {
            FaceWinding.Clockwise => Vd.FrontFace.Clockwise,
            FaceWinding.CounterClockwise => Vd.FrontFace.CounterClockwise,
            _ => throw new ArgumentOutOfRangeException(nameof(winding)),
        };
    }

    public static Vd.GraphicsBackend ToVeldrid(this GraphicsAPI graphicsAPI)
    {
        return graphicsAPI switch
        {
            GraphicsAPI.Direct3D11 => Vd.GraphicsBackend.Direct3D11,
            GraphicsAPI.Vulkan => Vd.GraphicsBackend.Vulkan,
            GraphicsAPI.OpenGL => Vd.GraphicsBackend.OpenGL,
            GraphicsAPI.Metal => Vd.GraphicsBackend.Metal,
            GraphicsAPI.OpenGLES => Vd.GraphicsBackend.OpenGLES,
            _ => throw new ArgumentOutOfRangeException(nameof(graphicsAPI)),
        };
    }

    public static Vd.MapMode ToVeldrid(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Read => Vd.MapMode.Read,
            MapMode.Write => Vd.MapMode.Write,
            MapMode.ReadWrite => Vd.MapMode.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }

    public static Vd.TextureType ToVeldrid(this NativeTextureKind kind)
    {
        return kind switch
        {
            NativeTextureKind.Texture1D => Vd.TextureType.Texture1D,
            NativeTextureKind.Texture2D => Vd.TextureType.Texture2D,
            NativeTextureKind.Texture3D => Vd.TextureType.Texture3D,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    public static Vd.TextureSampleCount ToVeldrid(this NativeTextureSampleCount sampleCount)
    {
        return sampleCount switch
        {
            NativeTextureSampleCount.Count1 => Vd.TextureSampleCount.Count1,
            NativeTextureSampleCount.Count2 => Vd.TextureSampleCount.Count2,
            NativeTextureSampleCount.Count4 => Vd.TextureSampleCount.Count4,
            NativeTextureSampleCount.Count8 => Vd.TextureSampleCount.Count8,
            NativeTextureSampleCount.Count16 => Vd.TextureSampleCount.Count16,
            NativeTextureSampleCount.Count32 => Vd.TextureSampleCount.Count32,
            _ => throw new ArgumentOutOfRangeException(nameof(sampleCount)),
        };
    }

    public static Vd.TextureUsage ToVeldrid(this NativeTextureUsage usage)
    {
        return (Vd.TextureUsage)(byte)usage;
    }

    public static Vd.PixelFormat ToVeldrid(this PixelFormat format)
    {
        return (Vd.PixelFormat)(byte)format;
    }

    public static Vd.PolygonFillMode ToVeldrid(this PolygonFillMode mode)
    {
        return mode switch
        {
            PolygonFillMode.Solid => Vd.PolygonFillMode.Solid,
            PolygonFillMode.Wireframe => Vd.PolygonFillMode.Wireframe,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }

    public static Vd.PrimitiveTopology ToVeldrid(this PrimitiveTopology topology)
    {
        return topology switch
        {
            PrimitiveTopology.Triangles => Vd.PrimitiveTopology.TriangleList,
            PrimitiveTopology.TriangleStrip => Vd.PrimitiveTopology.TriangleStrip,
            PrimitiveTopology.Lines => Vd.PrimitiveTopology.LineList,
            PrimitiveTopology.LineStrip => Vd.PrimitiveTopology.LineStrip,
            PrimitiveTopology.Points => Vd.PrimitiveTopology.PointList,
            _ => throw new ArgumentOutOfRangeException(nameof(topology)),
        };
    }

    public static Vd.ResourceKind ToVeldrid(this ResourceKind kind)
    {
        return kind switch
        {
            ResourceKind.UniformBuffer => Vd.ResourceKind.UniformBuffer,
            ResourceKind.StructuredBufferReadOnly => Vd.ResourceKind.StructuredBufferReadOnly,
            ResourceKind.StructuredBufferReadWrite => Vd.ResourceKind.StructuredBufferReadWrite,
            ResourceKind.TextureReadOnly => Vd.ResourceKind.TextureReadOnly,
            ResourceKind.TextureReadWrite => Vd.ResourceKind.TextureReadWrite,
            ResourceKind.Sampler => Vd.ResourceKind.Sampler,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    public static Vd.SamplerAddressMode ToVeldrid(this SamplerAddressMode mode)
    {
        return mode switch
        {
            SamplerAddressMode.Wrap => Vd.SamplerAddressMode.Wrap,
            SamplerAddressMode.Mirror => Vd.SamplerAddressMode.Mirror,
            SamplerAddressMode.Clamp => Vd.SamplerAddressMode.Clamp,
            SamplerAddressMode.Border => Vd.SamplerAddressMode.Border,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }

    public static Vd.SamplerBorderColor ToVeldrid(this SamplerBorderColor color)
    {
        return color switch
        {
            SamplerBorderColor.TransparentBlack => Vd.SamplerBorderColor.TransparentBlack,
            SamplerBorderColor.OpaqueBlack => Vd.SamplerBorderColor.OpaqueBlack,
            SamplerBorderColor.OpaqueWhite => Vd.SamplerBorderColor.OpaqueWhite,
            _ => throw new ArgumentOutOfRangeException(nameof(color)),
        };
    }

    public static Vd.SamplerFilter ToVeldrid(this SamplerFilter filter)
    {
        return filter switch
        {
            SamplerFilter.MinPoint_MagPoint_MipPoint => Vd.SamplerFilter.MinPoint_MagPoint_MipPoint,
            SamplerFilter.MinPoint_MagPoint_MipLinear => Vd.SamplerFilter.MinPoint_MagPoint_MipLinear,
            SamplerFilter.MinPoint_MagLinear_MipPoint => Vd.SamplerFilter.MinPoint_MagLinear_MipPoint,
            SamplerFilter.MinPoint_MagLinear_MipLinear => Vd.SamplerFilter.MinPoint_MagLinear_MipLinear,
            SamplerFilter.MinLinear_MagPoint_MipPoint => Vd.SamplerFilter.MinLinear_MagPoint_MipPoint,
            SamplerFilter.MinLinear_MagPoint_MipLinear => Vd.SamplerFilter.MinLinear_MagPoint_MipLinear,
            SamplerFilter.MinLinear_MagLinear_MipPoint => Vd.SamplerFilter.MinLinear_MagLinear_MipPoint,
            SamplerFilter.MinLinear_MagLinear_MipLinear => Vd.SamplerFilter.MinLinear_MagLinear_MipLinear,
            SamplerFilter.Anisotropic => Vd.SamplerFilter.Anisotropic,
            _ => throw new ArgumentOutOfRangeException(nameof(filter)),
        };
    }

    public static Vd.ShaderConstantType ToVeldrid(this ShaderConstantType type)
    {
        return type switch
        {
            ShaderConstantType.Bool => Vd.ShaderConstantType.Bool,
            ShaderConstantType.UInt16 => Vd.ShaderConstantType.UInt16,
            ShaderConstantType.Int16 => Vd.ShaderConstantType.Int16,
            ShaderConstantType.UInt32 => Vd.ShaderConstantType.UInt32,
            ShaderConstantType.Int32 => Vd.ShaderConstantType.Int32,
            ShaderConstantType.UInt64 => Vd.ShaderConstantType.UInt64,
            ShaderConstantType.Int64 => Vd.ShaderConstantType.Int64,
            ShaderConstantType.Float => Vd.ShaderConstantType.Float,
            ShaderConstantType.Double => Vd.ShaderConstantType.Double,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    public static Vd.ShaderStages ToVeldrid(this ShaderStage stage)
    {
        return (Vd.ShaderStages)(byte)stage;
    }

    public static Vd.StencilOperation ToVeldrid(this StencilOperation operation)
    {
        return operation switch
        {
            StencilOperation.Zero => Vd.StencilOperation.Zero,
            StencilOperation.Invert => Vd.StencilOperation.Invert,
            StencilOperation.Keep => Vd.StencilOperation.Keep,
            StencilOperation.Replace => Vd.StencilOperation.Replace,
            StencilOperation.Increment => Vd.StencilOperation.DecrementAndClamp,
            StencilOperation.Decrement => Vd.StencilOperation.DecrementAndClamp,
            StencilOperation.IncrementWrapped => Vd.StencilOperation.IncrementAndWrap,
            StencilOperation.DecrementWrapped => Vd.StencilOperation.DecrementAndWrap,
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
    }

    public static Vd.VertexElementFormat ToVeldrid(this VertexElementFormat format)
    {
        return (Vd.VertexElementFormat)(byte)format;
    }

    public static Vd.ResourceLayoutElementOptions ToVeldrid(this LayoutElementFlags flags)
    {
        return (Vd.ResourceLayoutElementOptions)(byte)flags;
    }

    public static Vd.IndexFormat ToVeldrid(this IndexBufferFormat format)
    {
        return format switch
        {
            IndexBufferFormat.UInt16 => Vd.IndexFormat.UInt16,
            IndexBufferFormat.UInt32 => Vd.IndexFormat.UInt32,
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };
    }

    public static Vd.SpecializationConstant ToVeldrid(this ShaderConstant constant)
    {
        return new
        (
            constant.ID,
            constant.Type.ToVeldrid(),
            constant.Data
        );
    }

    public static Vd.BlendAttachmentDescription ToVeldrid(this BlendAttachmentDescription desc)
    {
        return new
        (
            desc.Enabled,
            desc.SourceColor.ToVeldrid(),
            desc.DestinationColor.ToVeldrid(),
            desc.Color.ToVeldrid(),
            desc.SourceAlpha.ToVeldrid(),
            desc.DestinationAlpha.ToVeldrid(),
            desc.Alpha.ToVeldrid()
        );
    }

    public static Vd.BlendStateDescription ToVeldrid(this BlendStateDescription desc)
    {
        return new
        (
            desc.Factor.ToVeldrid(),
            desc.Attachments.Select(a => a.ToVeldrid()).ToArray()
        );
    }

    public static Vd.BufferDescription ToVeldrid(this BufferDescription desc)
    {
        return new
        (
            desc.Size,
            desc.Usage.ToVeldrid(),
            desc.StructureByteStride
        );
    }

    public static Vd.ComputePipelineDescription ToVeldrid(this ComputePipelineDescription desc)
    {
        return new
        (
            ((VeldridShader)desc.Shader).Resource,
            desc.Layouts.Select(r => ((VeldridResourceLayout)r).Resource).ToArray(),
            desc.ThreadGroupSizeX,
            desc.ThreadGroupSizeY,
            desc.ThreadGroupSizeZ
        );
    }

    public static Vd.DepthStencilStateDescription ToVeldrid(this DepthStencilStateDescription desc)
    {
        return new
        (
            desc.DepthTest,
            desc.DepthWrite,
            desc.DepthComparison.ToVeldrid(),
            desc.StencilTest,
            desc.StencilFront.ToVeldrid(),
            desc.StencilBack.ToVeldrid(),
            desc.StencilReadMask,
            desc.StencilWriteMask,
            desc.StencilReference
        );
    }

    public static Vd.FramebufferDescription ToVeldrid(this FramebufferDescription desc)
    {
        return new
        (
            desc.DepthTarget?.ToVeldrid(),
            desc.ColorTargets.Select(t => t.ToVeldrid()).ToArray()
        );
    }

    public static Vd.FramebufferAttachmentDescription ToVeldrid(this FramebufferAttachment attach)
    {
        return new
        (
            ((VeldridNativeTexture)attach.Target).Resource,
            attach.ArrayLayer,
            attach.MipLevel
        );
    }

    public static Vd.GraphicsPipelineDescription ToVeldrid(this GraphicsPipelineDescription desc)
    {
        return new
        (
            desc.Blend.ToVeldrid(),
            desc.DepthStencil.ToVeldrid(),
            desc.Rasterizer.ToVeldrid(),
            desc.Topology.ToVeldrid(),
            desc.ShaderSet.ToVeldrid(),
            desc.Layouts.Select(r => ((VeldridResourceLayout)r).Resource).ToArray(),
            desc.Outputs.ToVeldrid(),
            Vd.ResourceBindingModel.Improved
        );
    }

    public static Vd.OutputDescription ToVeldrid(this OutputDescription desc)
    {
        return new
        (
            desc.DepthAttachment?.ToVeldrid(),
            desc.ColorAttachments.Select(a => a.ToVeldrid()).ToArray(),
            desc.SampleCount.ToVeldrid()
        );
    }

    public static Vd.OutputAttachmentDescription ToVeldrid(this OutputAttachmentDescription desc)
    {
        return new(desc.Format.ToVeldrid());
    }

    public static Vd.ResourceLayoutDescription ToVeldrid(this LayoutDescription desc)
    {
        return new(desc.Elements.Select(e => e.ToVeldrid()).ToArray());
    }

    public static Vd.ResourceLayoutElementDescription ToVeldrid(this LayoutElementDescription desc)
    {
        return new
        (
            desc.Name,
            desc.Kind.ToVeldrid(),
            desc.Stages.ToVeldrid(),
            desc.Flags.ToVeldrid()
        );
    }

    public static Vd.TextureDescription ToVeldrid(this NativeTextureDescription desc)
    {
        return new
        (
            desc.Width,
            desc.Height,
            desc.Depth,
            desc.MipLevels,
            desc.ArrayLayers,
            desc.Format.ToVeldrid(),
            desc.Usage.ToVeldrid(),
            desc.Kind.ToVeldrid(),
            desc.SampleCount.ToVeldrid()
        );
    }

    public static Vd.RasterizerStateDescription ToVeldrid(this RasterizerStateDescription desc)
    {
        return new
        (
            desc.Culling.ToVeldrid(),
            desc.FillMode.ToVeldrid(),
            desc.Winding.ToVeldrid(),
            desc.DepthClip,
            desc.ScissorTest
        );
    }

    public static Vd.ResourceSetDescription ToVeldrid(this ResourceSetDescription desc)
    {
        var bindables = new List<Vd.BindableResource>();

        for (int i = 0; i < desc.Resources.Count; i++)
        {
            var resource = desc.Resources[i];

            if (resource is VeldridNativeTexture texture)
                bindables.Add(texture.Resource);

            if (resource is VeldridBuffer buffer)
                bindables.Add(buffer.Resource);
        }

        return new
        (
            ((VeldridResourceLayout)desc.Layout).Resource,
            bindables.ToArray()
        );
    }

    public static Vd.SamplerDescription ToVeldrid(this SamplerDescription desc)
    {
        return new
        (
            desc.AddressModeU.ToVeldrid(),
            desc.AddressModeV.ToVeldrid(),
            desc.AddressModeW.ToVeldrid(),
            desc.Filter.ToVeldrid(),
            desc.Comparison?.ToVeldrid(),
            desc.MaximumAnisotropy,
            desc.MinimumLod,
            desc.MaximumLod,
            desc.LodBias,
            desc.Border.ToVeldrid()
        );
    }

    public static Vd.ShaderDescription ToVeldrid(this ShaderDescription desc)
    {
        return new
        (
            desc.Stage.ToVeldrid(),
            desc.Code,
            desc.EntryPoint
        );
    }

    public static Vd.ShaderSetDescription ToVeldrid(this ShaderSetDescription desc)
    {
        return new
        (
            desc.Layouts.Select(v => v.ToVeldrid()).ToArray(),
            desc.Shaders.Select(s => ((VeldridShader)s).Resource).ToArray(),
            desc.Constants.Select(c => c.ToVeldrid()).ToArray()
        );
    }

    public static Vd.StencilBehaviorDescription ToVeldrid(this StencilBehaviorDescription desc)
    {
        return new
        (
            desc.Fail.ToVeldrid(),
            desc.Pass.ToVeldrid(),
            desc.DepthFail.ToVeldrid(),
            desc.Comparison.ToVeldrid()
        );
    }

    public static Vd.SwapchainDescription ToVeldrid(this SwapChainDescription desc)
    {
        return new
        (
            desc.Source.Create(),
            desc.Width,
            desc.Height,
            desc.DepthTargetFormat?.ToVeldrid(),
            desc.VerticalSync,
            true
        );
    }

    public static Vd.SwapchainSource Create(this SwapChainSource source)
    {
        switch (source.Source.Kind)
        {
            case Framework.Windowing.NativeWindowKind.DirectFramebuffer:
            case Framework.Windowing.NativeWindowKind.Unknown:
            case Framework.Windowing.NativeWindowKind.Vivante:
            case Framework.Windowing.NativeWindowKind.Android:
            case Framework.Windowing.NativeWindowKind.Haiku:
            case Framework.Windowing.NativeWindowKind.UIKit:
            case Framework.Windowing.NativeWindowKind.WinRT:
            case Framework.Windowing.NativeWindowKind.OS2:
            default:
                throw new PlatformNotSupportedException();

            case Framework.Windowing.NativeWindowKind.Win32:
                {
                    if (!source.Source.Win32.HasValue)
                        throw new InvalidOperationException();

                    return Vd.SwapchainSource.CreateWin32(source.Source.Win32.Value.Hwnd, source.Source.Win32.Value.HInstance);
                }

            case Framework.Windowing.NativeWindowKind.X11:
                {
                    if (!source.Source.X11.HasValue)
                        throw new InvalidOperationException();

                    return Vd.SwapchainSource.CreateXlib(source.Source.X11.Value.Display, source.Source.X11.Value.Window);
                }

            case Framework.Windowing.NativeWindowKind.Wayland:
                {
                    if (!source.Source.Wayland.HasValue)
                        throw new InvalidOperationException();

                    return Vd.SwapchainSource.CreateXlib(source.Source.Wayland.Value.Display, source.Source.Wayland.Value.Surface);
                }
            case Framework.Windowing.NativeWindowKind.Cocoa:
            {
                if (!source.Source.Cocoa.HasValue)
                    throw new InvalidOperationException();
                return Vd.SwapchainSource.CreateNSWindow(source.Source.Cocoa.Value);
            }
        }
    }

    public static Vd.VertexElementDescription ToVeldrid(this VertexElementDescription desc)
    {
        return new
        (
            desc.Name,
            Vd.VertexElementSemantic.TextureCoordinate,
            desc.Format.ToVeldrid(),
            desc.Offset
        );
    }

    public static Vd.VertexLayoutDescription ToVeldrid(this VertexLayoutDescription desc)
    {
        return new
        (
            desc.Stride,
            desc.InstanceStepRate,
            desc.Elements.Select(e => e.ToVeldrid()).ToArray()
        );
    }

    public static Vd.Viewport ToVeldrid(this Viewport viewport)
    {
        return new
        (
            viewport.X,
            viewport.Y,
            viewport.Width,
            viewport.Height,
            viewport.MinimumDepth,
            viewport.MaximumDepth
        );
    }
}
