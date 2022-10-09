// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Windowing;

namespace Sekai.Dummy;

internal class DummyGraphicsDevice : FrameworkObject, IGraphicsDevice
{
    public string Name => string.Empty;
    public GraphicsAPI GraphicsAPI => GraphicsAPI.OpenGL;
    public ISwapChain SwapChain { get; } = null!;
    public GraphicsDeviceFeatures Features { get; } = new GraphicsDeviceFeatures();
    public IGraphicsResourceFactory Factory { get; } = new DummyGraphicsResourceFactory();
    public ITexture WhitePixel { get; }
    public ISampler SamplerPoint { get; }
    public ISampler SamplerLinear { get; }
    public ISampler SamplerAniso4x { get; }

    public DummyGraphicsDevice()
    {
        var descriptor = new SwapChainDescription();
        SwapChain = Factory.CreateSwapChain(ref descriptor);

        WhitePixel = new DummyTexture
        (
            PixelFormat.R8_G8_B8_A8_UNorm,
            1,
            1,
            1,
            0,
            0,
            TextureUsage.Sampled,
            TextureKind.Texture2D,
            TextureSampleCount.Count1
        );

        SamplerPoint = SamplerLinear = SamplerAniso4x = new DummySampler();
    }

    public ShaderCompilationResult CompileShader(string source, ShaderStage stage, ShaderCompilationOptions? options = null)
    {
        return new ShaderCompilationResult();
    }

    public void Initialize(IView view, GraphicsContextOptions graphicsAPI)
    {
    }

    public MappedResource Map(IBuffer buffer, MapMode mode)
    {
        return new MappedResource(buffer, mode, IntPtr.Zero, 0, 0, 0, 0);
    }

    public MappedResource Map(ITexture texture, MapMode mode, uint subResource)
    {
        return new MappedResource(texture, mode, IntPtr.Zero, 0, 0, 0, 0);
    }

    public void Submit(ICommandQueue queue)
    {
    }

    public void SwapBuffers()
    {
    }

    public void SwapBuffers(ISwapChain swapChain)
    {
    }

    public void Unmap(IBuffer buffer)
    {
    }

    public void Unmap(ITexture texture, uint subResource)
    {
    }

    public void UpdateBufferData(IBuffer buffer, nint source, uint sourceSizeInBytes, uint offset)
    {
    }

    public void UpdateBufferData<T>(IBuffer buffer, ref T data, uint offset) where T : struct
    {
    }

    public void UpdateBufferData<T>(IBuffer buffer, T[] data, uint offset) where T : struct
    {
    }

    public void UpdateTextureData(ITexture texture, nint source, uint size, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer)
    {
    }

    public void UpdateTextureData<T>(ITexture texture, T[] data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) where T : struct
    {
    }
}
