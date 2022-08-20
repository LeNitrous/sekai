// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Windowing;

namespace Sekai.Headless;

internal class HeadlessGraphicsContext : FrameworkObject, IGraphicsContext
{
    public IGraphicsResourceFactory Factory { get; } = new HeadlessGraphicsResourceFactory();

    public void Initialize(IView view, GraphicsContextOptions graphicsAPI)
    {
    }

    public MappedResource Map(IBuffer buffer, MapMode mode)
    {
        return new MappedResource(buffer, mode, IntPtr.Zero, 0, 0, 0, 0);
    }

    public MappedResource Map(INativeTexture texture, MapMode mode, uint subResource)
    {
        return new MappedResource(texture, mode, IntPtr.Zero, 0, 0, 0, 0);
    }

    public void Submit(ICommandQueue queue)
    {
    }

    public void Unmap(IBuffer buffer)
    {
    }

    public void Unmap(INativeTexture texture, uint subResource)
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

    public void UpdateTextureData(INativeTexture texture, nint source, uint size, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer)
    {
    }

    public void UpdateTextureData<T>(INativeTexture texture, T[] data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) where T : struct
    {
    }
}
