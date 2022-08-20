// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessGraphicsResourceFactory : IGraphicsResourceFactory
{
    public IBuffer CreateBuffer(ref BufferDescription description)
    {
        return new HeadlessBuffer(description.Size, description.Usage);
    }

    public IBuffer CreateBuffer(nint pointer, ref BufferDescription description)
    {
        return new HeadlessBuffer(description.Size, description.Usage);
    }

    public IBuffer CreateBuffer<T>(ref T data, ref BufferDescription description)
        where T : struct
    {
        return new HeadlessBuffer(description.Size, description.Usage);
    }

    public IBuffer CreateBuffer<T>(T[] data, ref BufferDescription description)
        where T : struct
    {
        return new HeadlessBuffer(description.Size, description.Usage);
    }

    public ICommandQueue CreateCommandQueue()
    {
        return new HeadlessCommandQueue();
    }

    public IPipeline CreatePipeline(ref ComputePipelineDescription description)
    {
        return new HeadlessPipeline(PipelineKind.Compute);
    }

    public IPipeline CreatePipeline(ref GraphicsPipelineDescription description)
    {
        return new HeadlessPipeline(PipelineKind.Graphics);
    }

    public IFramebuffer CreateFramebuffer(ref FramebufferDescription description)
    {
        return new HeadlessFramebuffer(description);
    }

    public INativeTexture CreateNativeTexture(ref NativeTextureDescription description)
    {
        return new HeadlessNativeTexture
        (
            description.Format,
            description.Width,
            description.Height,
            description.Depth,
            description.MipLevels,
            description.ArrayLayers,
            description.Usage,
            description.Kind,
            description.SampleCount
        );
    }

    public INativeTexture CreateNativeTexture(nint pointer, ref NativeTextureDescription description)
    {
        return new HeadlessNativeTexture
        (
            description.Format,
            description.Width,
            description.Height,
            description.Depth,
            description.MipLevels,
            description.ArrayLayers,
            description.Usage,
            description.Kind,
            description.SampleCount
        );
    }

    public IResourceLayout CreateResourceLayout(ref LayoutDescription description)
    {
        return new HeadlessResourceLayout();
    }

    public IResourceSet CreateResourceSet(ref ResourceSetDescription description)
    {
        return new HeadlessResourceSet();
    }

    public ISampler CreateSampler(ref SamplerDescription description)
    {
        return new HeadlessSampler();
    }

    public IShader CreateShader(ref ShaderDescription description)
    {
        return new HeadlessShader(description.Stage, description.EntryPoint);
    }

    public ISwapChain CreateSwapChain(ref SwapChainDescription description)
    {
        var framebufferDescription = new FramebufferDescription
        (
            !description.DepthTargetFormat.HasValue
                ? null
                : new FramebufferAttachment
                (
                    new HeadlessNativeTexture
                    (
                        PixelFormat.D32_Float_S8_UInt,
                        description.Width,
                        description.Height,
                        0,
                        1,
                        1,
                        NativeTextureUsage.DepthStencil,
                        NativeTextureKind.Texture2D,
                        NativeTextureSampleCount.Count16
                    ),
                    1,
                    1
                ),
            new[]
            {
                new FramebufferAttachment
                (
                    new HeadlessNativeTexture
                    (
                        PixelFormat.B8_G8_R8_A8_UNorm_SRgb,
                        description.Width,
                        description.Height,
                        0,
                        1,
                        1,
                        NativeTextureUsage.RenderTarget,
                        NativeTextureKind.Texture2D,
                        NativeTextureSampleCount.Count16
                    ),
                    1,
                    1
                ),
            }
        );

        return new HeadlessSwapChain(CreateFramebuffer(ref framebufferDescription), description.VerticalSync);
    }
}
