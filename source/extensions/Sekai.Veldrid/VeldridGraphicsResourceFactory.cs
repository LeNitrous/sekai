// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridGraphicsResourceFactory : FrameworkObject, IGraphicsResourceFactory
{
    private readonly Vd.ResourceFactory factory;
    private readonly VeldridGraphicsDevice context;

    public VeldridGraphicsResourceFactory(VeldridGraphicsDevice context, Vd.ResourceFactory factory)
    {
        this.factory = factory;
        this.context = context;
    }

    public IBuffer CreateBuffer(ref BufferDescription description)
    {
        return new VeldridBuffer(description, factory.CreateBuffer(description.ToVeldrid()));
    }

    public IBuffer CreateBuffer(nint pointer, ref BufferDescription description)
    {
        var buffer = CreateBuffer(ref description);
        context.UpdateBufferData(buffer, pointer, buffer.Size, 0);
        return buffer;
    }

    public IBuffer CreateBuffer<T>(ref T data, ref BufferDescription description)
        where T : struct
    {
        var buffer = CreateBuffer(ref description);
        context.UpdateBufferData(buffer, ref data, 0);
        return buffer;
    }

    public IBuffer CreateBuffer<T>(T[] data, ref BufferDescription description)
        where T : struct
    {
        var buffer = CreateBuffer(ref description);
        context.UpdateBufferData(buffer, data, 0);
        return buffer;
    }

    public ICommandQueue CreateCommandQueue()
    {
        return new VeldridCommandQueue(factory.CreateCommandList());
    }

    public IFramebuffer CreateFramebuffer(ref FramebufferDescription description)
    {
        return new VeldridFramebuffer(description, factory.CreateFramebuffer(description.ToVeldrid()));
    }

    public ITexture CreateTexture(ref TextureDescription description)
    {
        return new VeldridTexture(description, factory.CreateTexture(description.ToVeldrid()));
    }

    public ITexture CreateTexture(nint pointer, ref TextureDescription description)
    {
        return new VeldridTexture(description, factory.CreateTexture((ulong)pointer, description.ToVeldrid()));
    }

    public IPipeline CreatePipeline(ref ComputePipelineDescription description)
    {
        return new VeldridPipeline(PipelineKind.Compute, factory.CreateComputePipeline(description.ToVeldrid()));
    }

    public IPipeline CreatePipeline(ref GraphicsPipelineDescription description)
    {
        return new VeldridPipeline(PipelineKind.Graphics, factory.CreateGraphicsPipeline(description.ToVeldrid()));
    }

    public IResourceLayout CreateResourceLayout(ref LayoutDescription description)
    {
        return new VeldridResourceLayout(factory.CreateResourceLayout(description.ToVeldrid()));
    }

    public IResourceSet CreateResourceSet(ref ResourceSetDescription description)
    {
        return new VeldridResourceSet(factory.CreateResourceSet(description.ToVeldrid()));
    }

    public ISampler CreateSampler(ref SamplerDescription description)
    {
        return new VeldridSampler(factory.CreateSampler(description.ToVeldrid()));
    }

    public IShader CreateShader(ref ShaderDescription description)
    {
        return new VeldridShader(description, factory.CreateShader(description.ToVeldrid()));
    }

    public ISwapChain CreateSwapChain(ref SwapChainDescription description)
    {
        return new VeldridSwapChain(factory.CreateSwapchain(description.ToVeldrid()));
    }
}
