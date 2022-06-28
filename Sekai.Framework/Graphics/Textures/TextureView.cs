// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Framework.Graphics.Textures;

public class TextureView : GraphicsObject<Veldrid.TextureView>, IBindableResource
{
    public readonly Texture Target;
    public readonly PixelFormat Format;
    public readonly int BaseMipLevel;
    public readonly int MipLevels;
    public readonly int BaseLayer;
    public readonly int Layer;
    internal override Veldrid.TextureView Resource { get; }

    public TextureView(Texture target)
        : this(target, target.Format)
    {
    }

    public TextureView(Texture target, PixelFormat format)
        : this(target, format, 0, target.MipLevels, 0, target.Layers)
    {
    }

    public TextureView(Texture target, PixelFormat format, int baseMipLevel, int mipLevels, int baseLayer, int layer)
    {
        Target = target;
        Format = format;
        BaseMipLevel = baseMipLevel;
        MipLevels = mipLevels;
        BaseLayer = baseLayer;
        Layer = layer;
        Resource = Context.Resources.CreateTextureView(new TextureViewDescription(Target.Resource, (uint)BaseMipLevel, (uint)MipLevels, (uint)BaseLayer, (uint)Layer));
    }

    BindableResource IBindableResource.Resource => Resource;
}
