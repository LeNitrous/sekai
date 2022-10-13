// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Windowing;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal partial class VeldridGraphicsDevice : FrameworkObject, IGraphicsDevice
{
    public string Name { get; private set; } = string.Empty;
    public ISwapChain SwapChain { get; private set; } = null!;
    public GraphicsAPI GraphicsAPI { get; private set; } = GraphicsAPI.OpenGL;
    public GraphicsDeviceFeatures Features { get; private set; } = null!;
    public IGraphicsResourceFactory Factory { get; private set; } = null!;
    public ITexture WhitePixel { get; private set; } = null!;
    public ISampler SamplerPoint { get; private set; } = null!;
    public ISampler SamplerLinear { get; private set; } = null!;
    public ISampler SamplerAniso4x { get; private set; } = null!;
    private Vd.GraphicsDevice device = null!;
    private bool vsync;

    public bool VerticalSync
    {
        get => vsync;
        set
        {
            if (device != null)
                device.SyncToVerticalBlank = value;

            vsync = value;
        }
    }

    public void Initialize(IView view, GraphicsContextOptions options)
    {
        if (view is not INativeWindowSource windowSource)
            throw new InvalidOperationException(@"View must provide a native window for this graphics context to initialize.");

        VerticalSync = options.VerticalSync;

        var swapChainDescription = new SwapChainDescription
        (
            new SwapChainSource(windowSource.Native),
            options.DepthTargetFormat,
            (uint)view.Size.Width,
            (uint)view.Size.Height,
            VerticalSync,
            options.ColorSRGB
        ).ToVeldrid();

        if (!options.GraphicsAPI.HasValue)
            throw new InvalidOperationException(@"There must be a non-null graphics API to be used.");

        var graphicsDeviceOptions = new Vd.GraphicsDeviceOptions
        (
            options.Debug ?? false,
            options.DepthTargetFormat?.ToVeldrid(),
            VerticalSync,
            Vd.ResourceBindingModel.Improved,
            true,
            true
        );

        switch (options.GraphicsAPI.Value)
        {
            case GraphicsAPI.Direct3D11:
                initializeDirect3D11(graphicsDeviceOptions, swapChainDescription);
                break;

            case GraphicsAPI.OpenGL:
                initializeOpenGL(view, graphicsDeviceOptions, swapChainDescription);
                break;

            case GraphicsAPI.OpenGLES:
                initializeOpenGLES(graphicsDeviceOptions, swapChainDescription);
                break;

            case GraphicsAPI.Vulkan:
                initializeVulkan(graphicsDeviceOptions, swapChainDescription);
                break;

            case GraphicsAPI.Metal:
                initializeMetal(graphicsDeviceOptions, swapChainDescription);
                break;
        }

        SwapChain = new VeldridSwapChain(device.MainSwapchain);

        Factory = new VeldridGraphicsResourceFactory(this, device.ResourceFactory);

        Features = new GraphicsDeviceFeatures
        {
            ComputeShaders = device.Features.ComputeShader,
            FillModeWireframe = device.Features.FillModeWireframe,
            SamplerAnisotropy = device.Features.SamplerAnisotropy,
        };

        if (view is IWindow window)
            window.OnResize += s => device.ResizeMainWindow((uint)s.Width, (uint)s.Height);

        byte[] whitePixelData = new byte[] { 255, 255, 255, 255 };
        var whitePixelDescriptor = new TextureDescription
        (
            1,
            1,
            1,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureKind.Texture2D,
            TextureUsage.Sampled,
            TextureSampleCount.Count1
        );

        WhitePixel = Factory.CreateTexture(ref whitePixelDescriptor);
        UpdateTextureData(WhitePixel, whitePixelData, 0, 0, 0, 1, 1, 1, 0, 0);

        var samplerDescriptor = new SamplerDescription
        (
            SamplerAddressMode.Wrap,
            SamplerAddressMode.Wrap,
            SamplerAddressMode.Wrap,
            SamplerFilter.MinPoint_MagPoint_MipPoint,
            null,
            0,
            0,
            uint.MaxValue,
            0,
            SamplerBorderColor.TransparentBlack
        );

        SamplerPoint = Factory.CreateSampler(ref samplerDescriptor);

        samplerDescriptor.Filter = SamplerFilter.MinLinear_MagLinear_MipLinear;
        SamplerLinear = Factory.CreateSampler(ref samplerDescriptor);

        samplerDescriptor.Filter = SamplerFilter.Anisotropic;
        samplerDescriptor.MaximumAnisotropy = 4;
        SamplerAniso4x = Factory.CreateSampler(ref samplerDescriptor);
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

    public MappedResource Map(ITexture texture, MapMode mode, uint subResource)
    {
        var resource = device.Map(((VeldridTexture)texture).Resource, mode.ToVeldrid());
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

    public void Unmap(ITexture texture, uint subResource)
    {
        device.Unmap(((VeldridTexture)texture).Resource, subResource);
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

    public void UpdateTextureData(ITexture texture, nint source, uint size, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer)
    {
        device.UpdateTexture(((VeldridTexture)texture).Resource, source, size, x, y, z, width, height, depth, mipLevel, arrayLayer);
    }

    public void UpdateTextureData<T>(ITexture texture, T[] data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer)
        where T : struct
    {
        device.UpdateTexture(((VeldridTexture)texture).Resource, data, x, y, z, width, height, depth, mipLevel, arrayLayer);
    }

    public void SwapBuffers()
    {
        device.SwapBuffers();
    }

    public void SwapBuffers(ISwapChain swapChain)
    {
        device.SwapBuffers(((VeldridSwapChain)swapChain).Resource);
    }

    protected sealed override void Destroy()
    {
        device.Dispose();
    }
}
