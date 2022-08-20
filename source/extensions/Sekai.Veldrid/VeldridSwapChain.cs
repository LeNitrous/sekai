using System.Linq;
using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridSwapChain : VeldridGraphicsResource<Vd.Swapchain>, ISwapChain
{
    public IFramebuffer Framebuffer { get; }

    public bool VerticalSync
    {
        get => Resource.SyncToVerticalBlank;
        set => Resource.SyncToVerticalBlank = value;
    }

    public VeldridSwapChain(SwapChainDescription desc, Vd.Swapchain resource)
        : base(resource)
    {
        Framebuffer = new VeldridFramebuffer
        (
            new FramebufferDescription
            (
                resource.Framebuffer.DepthTarget.HasValue ? fromVeldrid(resource.Framebuffer.DepthTarget.Value) : null,
                resource.Framebuffer.ColorTargets.Select(fromVeldrid).ToArray()
            ),
            resource.Framebuffer
        );
    }

    private static FramebufferAttachment fromVeldrid(Vd.FramebufferAttachment attach)
    {
        return new FramebufferAttachment
        (
            new VeldridNativeTexture
            (
                new NativeTextureDescription
                (
                    attach.Target.Width,
                    attach.Target.Height,
                    attach.Target.Depth,
                    attach.Target.MipLevels,
                    attach.Target.ArrayLayers,
                    (PixelFormat)attach.Target.Format,
                    (NativeTextureKind)attach.Target.Type,
                    (NativeTextureUsage)attach.Target.Usage,
                    (NativeTextureSampleCount)attach.Target.SampleCount
                ),
                attach.Target
            ),
            attach.ArrayLayer,
            attach.MipLevel
        );
    }
}
