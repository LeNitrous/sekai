// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// A factory for creating GPU resources.
/// </summary>
public interface IGraphicsResourceFactory
{
    /// <summary>
    /// Creates a buffer.
    /// </summary>
    IBuffer CreateBuffer(ref BufferDescription description);

    /// <summary>
    /// Creates a buffer.
    /// </summary>
    IBuffer CreateBuffer(nint pointer, ref BufferDescription description);

    /// <summary>
    /// Creates a buffer.
    /// </summary>
    IBuffer CreateBuffer<T>(ref T data, ref BufferDescription description) where T: struct;

    /// <summary>
    /// Creates a buffer.
    /// </summary>
    IBuffer CreateBuffer<T>(T[] data, ref BufferDescription description) where T: struct;

    /// <summary>
    /// Creates a command queue.
    /// </summary>
    ICommandQueue CreateCommandQueue();

    /// <summary>
    /// Creates a compute pipeline.
    /// </summary>
    IPipeline CreatePipeline(ref ComputePipelineDescription description);

    /// <summary>
    /// Creates a graphics pipeline.
    /// </summary>
    IPipeline CreatePipeline(ref GraphicsPipelineDescription description);

    /// <summary>
    /// Creates a framebuffer.
    /// </summary>
    IFramebuffer CreateFramebuffer(ref FramebufferDescription description);

    /// <summary>
    /// Creates a resource layout.
    /// </summary>
    IResourceLayout CreateResourceLayout(ref LayoutDescription description);

    /// <summary>
    /// Creates a resource set.
    /// </summary>
    IResourceSet CreateResourceSet(ref ResourceSetDescription description);

    /// <summary>
    /// Creates a sampler.
    /// </summary>
    ISampler CreateSampler(ref SamplerDescription description);

    /// <summary>
    /// Creates a shader.
    /// </summary>
    IShader CreateShader(ref ShaderDescription description);

    /// <summary>
    /// Creates a native texture.
    /// </summary>
    INativeTexture CreateNativeTexture(ref NativeTextureDescription description);

    /// <summary>
    /// Creates a native texture from an existing pointer.
    /// </summary>
    INativeTexture CreateNativeTexture(nint pointer, ref NativeTextureDescription description);

    /// <summary>
    /// Creates a swap chain.
    /// </summary>
    ISwapChain CreateSwapChain(ref SwapChainDescription description);
}
