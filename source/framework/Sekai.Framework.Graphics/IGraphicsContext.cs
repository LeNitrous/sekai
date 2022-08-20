// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Windowing;

namespace Sekai.Framework.Graphics;

public interface IGraphicsContext : IDisposable
{
    IGraphicsResourceFactory Factory { get; }
    void Initialize(IView view, GraphicsContextOptions options);
    MappedResource Map(IBuffer buffer, MapMode mode);
    MappedResource Map(INativeTexture texture, MapMode mode, uint subResource);
    void Unmap(IBuffer buffer);
    void Unmap(INativeTexture texture, uint subResource);
    void UpdateBufferData(IBuffer buffer, nint source, uint sourceSizeInBytes, uint offset);
    void UpdateBufferData<T>(IBuffer buffer, ref T data, uint offset) where T : struct;
    void UpdateBufferData<T>(IBuffer buffer, T[] data, uint offset) where T : struct;
    void UpdateTextureData(INativeTexture texture, nint source, uint size, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer);
    void UpdateTextureData<T>(INativeTexture texture, T[] data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) where T : struct;
    void Submit(ICommandQueue queue);
}
