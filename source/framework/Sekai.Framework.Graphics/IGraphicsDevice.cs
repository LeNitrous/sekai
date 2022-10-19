// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Windowing;

namespace Sekai.Framework.Graphics;

/// <summary>
/// The graphics device.
/// </summary>
public interface IGraphicsDevice : IDisposable
{
    /// <summary>
    /// The name of this device.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets or sets vertical syncing for this device.
    /// </summary>
    bool VerticalSync { get; set; }

    /// <summary>
    /// The graphics API used by this device.
    /// </summary>
    GraphicsAPI GraphicsAPI { get; }

    /// <summary>
    /// Gets the supported features for this device.
    /// </summary>
    GraphicsDeviceFeatures Features { get; }

    /// <summary>
    /// The resource factory used to create graphics resources.
    /// </summary>
    IGraphicsResourceFactory Factory { get; }

    /// <summary>
    /// The main swap chain for this device.
    /// </summary>
    ISwapChain SwapChain { get; }

    /// <summary>
    /// A white pixel as a texture.
    /// </summary>
    ITexture WhitePixel { get; }

    /// <summary>
    /// Point-filtered sampler.
    /// </summary>
    ISampler SamplerPoint { get; }

    /// <summary>
    /// Linear-filtered sampler.
    /// </summary>
    ISampler SamplerLinear { get; }

    /// <summary>
    /// 4x anisotropic-filtered sampler.
    /// </summary>
    ISampler SamplerAniso4x { get; }

    /// <summary>
    /// Initializes this device.
    /// </summary>
    void Initialize(IView view, GraphicsContextOptions options);

    /// <summary>
    /// Maps a buffer making it visible to CPU.
    /// </summary>
    MappedResource Map(IBuffer buffer, MapMode mode);

    /// <summary>
    /// Maps a texture making it visible to the CPU.
    /// </summary>
    MappedResource Map(ITexture texture, MapMode mode, uint subResource);

    /// <summary>
    /// Unmaps a buffer.
    /// </summary>
    void Unmap(IBuffer buffer);

    /// <summary>
    /// Unmaps a texture.
    /// </summary>
    void Unmap(ITexture texture, uint subResource);

    /// <summary>
    /// Updates buffer data using a pointer.
    /// </summary>
    void UpdateBufferData(IBuffer buffer, nint source, uint sourceSizeInBytes, uint offset);

    /// <summary>
    /// Updates buffer data with typed data.
    /// </summary>
    void UpdateBufferData<T>(IBuffer buffer, ref T data, uint offset) where T : struct;

    /// <summary>
    /// Updates buffer data with typed data.
    /// </summary>
    void UpdateBufferData<T>(IBuffer buffer, T[] data) where T : struct;

    /// <summary>
    /// Updates buffer data with typed data.
    /// </summary>
    void UpdateBufferData<T>(IBuffer buffer, Span<T> data) where T : unmanaged;

    /// <summary>
    /// Updates a texture region using a pointer.
    /// </summary>
    void UpdateTextureData(ITexture texture, nint source, uint size, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer);

    /// <summary>
    /// Updates a texture region with typed data.
    /// </summary>
    void UpdateTextureData<T>(ITexture texture, T[] data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) where T : struct;

    /// <summary>
    /// Updates a texture region with typed data.
    /// </summary>
    void UpdateTextureData<T>(ITexture texture, Span<T> data, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) where T : unmanaged;

    /// <summary>
    /// Submits a command queue.
    /// </summary>
    void Submit(ICommandQueue queue);

    /// <summary>
    /// Swaps buffers of the main swapchain.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// Swaps the given swap chain's buffers.
    /// </summary>
    void SwapBuffers(ISwapChain swapChain);

    /// <summary>
    /// Compiles a given GLSL shader source.
    /// </summary>
    ShaderCompilationResult CompileShader(string source, ShaderStage stage, ShaderCompilationOptions? options = null);
}
