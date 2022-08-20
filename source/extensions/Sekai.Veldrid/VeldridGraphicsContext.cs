// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Windowing;
using Sekai.Framework.Windowing.OpenGL;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridGraphicsContext : FrameworkObject, IGraphicsContext
{
    public IGraphicsResourceFactory Factory { get; private set; } = null!;
    private Vd.GraphicsDevice device = null!;

    public void Initialize(IView view, GraphicsContextOptions options)
    {
        if (view is not INativeWindowSource windowSource)
            throw new InvalidOperationException(@"View must provide a native window for this graphics context to initialize.");

        var swapChainDescription = new SwapChainDescription
        (
            new SwapChainSource(windowSource.Native),
            options.DepthTargetFormat,
            (uint)view.Size.Width,
            (uint)view.Size.Height,
            options.VerticalSync,
            options.ColorSRGB
        ).ToVeldrid();

        if (!options.GraphicsAPI.HasValue)
            throw new InvalidOperationException(@"There must be a non-null graphics API to be used.");

        var graphicsDeviceOptions = new Vd.GraphicsDeviceOptions
        (
            options.Debug ?? false,
            options.DepthTargetFormat?.ToVeldrid(),
            options.VerticalSync,
            Vd.ResourceBindingModel.Improved,
            true,
            true
        );

        switch (options.GraphicsAPI.Value)
        {
            case GraphicsAPI.Direct3D11:
                {
                    device = Vd.GraphicsDevice.CreateD3D11(graphicsDeviceOptions, swapChainDescription);
                    break;
                }

            case GraphicsAPI.OpenGL:
                {
                    if (view is not IOpenGLProviderSource glSource)
                        throw new InvalidOperationException(@"The view must be able to provide an OpenGL context.");

                    var gl = glSource.GL;

                    var openGLPlatformOptions = new Vd.OpenGL.OpenGLPlatformInfo
                    (
                        gl.Handle,
                        gl.GetProcAddress,
                        gl.MakeCurrent,
                        gl.GetCurrentContext,
                        gl.ClearCurrentContext,
                        gl.DeleteContext,
                        gl.SwapBuffers,
                        gl.SetSyncToVerticalBlank
                    );

                    device = Vd.GraphicsDevice.CreateOpenGL(graphicsDeviceOptions, openGLPlatformOptions, (uint)view.Size.Width, (uint)view.Size.Height);
                    break;
                }

            case GraphicsAPI.OpenGLES:
                {
                    device = Vd.GraphicsDevice.CreateOpenGLES(graphicsDeviceOptions, swapChainDescription);
                    break;
                }

            case GraphicsAPI.Vulkan:
                {
                    device = Vd.GraphicsDevice.CreateVulkan(graphicsDeviceOptions, swapChainDescription);
                    break;
                }

            case GraphicsAPI.Metal:
                {
                    device = Vd.GraphicsDevice.CreateMetal(graphicsDeviceOptions, swapChainDescription);
                    break;
                }
        }

        Factory = new VeldridGraphicsResourceFactory(this, device.ResourceFactory);
    }

    public MappedResource Map(IBuffer buffer, MapMode mode)
    {
        var resource = device.Map(((VeldridBuffer)buffer).Resource, mode.ToVeldrid());
        return new MappedResource
        (
            buffer,
            mode,
            resource.Data,
            resource.SizeInBytes,
            resource.Subresource,
            resource.DepthPitch,
            resource.RowPitch
        );
    }

    public MappedResource Map(INativeTexture texture, MapMode mode, uint subResource)
    {
        var resource = device.Map(((VeldridNativeTexture)texture).Resource, mode.ToVeldrid());
        return new MappedResource
        (
            texture,
            mode,
            resource.Data,
            resource.SizeInBytes,
            resource.Subresource,
            resource.DepthPitch,
            resource.RowPitch
        );
    }

    public void Submit(ICommandQueue queue)
    {
        device.SubmitCommands(((VeldridCommandQueue)queue).Resource);
    }

    public void Unmap(IBuffer buffer)
    {
        device.Unmap(((VeldridBuffer)buffer).Resource);
    }

    public void Unmap(INativeTexture texture, uint subResource)
    {
         device.Unmap(((VeldridNativeTexture)texture).Resource, subResource);
    }

    public void UpdateBufferData(IBuffer buffer, nint source, uint offset, uint size)
    {
        device.UpdateBuffer(((VeldridBuffer)buffer).Resource, offset, source, size);
    }

    public void UpdateBufferData<T>(IBuffer buffer, ref T data, uint offset)
        where T : struct
    {
        device.UpdateBuffer(((VeldridBuffer)buffer).Resource, offset, ref data);
    }

    public void UpdateBufferData<T>(IBuffer buffer, T[] data, uint offset)
        where T : struct
    {
        device.UpdateBuffer(((VeldridBuffer)buffer).Resource, offset, data);
    }

    public void UpdateTextureData(INativeTexture texture, nint source, uint size, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer)
    {
        device.UpdateTexture(((VeldridNativeTexture)texture).Resource, source, size, x, y, z, width, height, depth, mipLevel, arrayLayer);
    }

    public void UpdateTextureData<T>(INativeTexture texture, T[] data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer)
        where T : struct
    {
        device.UpdateTexture(((VeldridNativeTexture)texture).Resource, data, x, y, z, width, height, depth, mipLevel, arrayLayer);
    }
}
