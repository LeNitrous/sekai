// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Sekai.Framework.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Sekai.Framework.Graphics.Textures;

/// <summary>
/// A <see cref="GraphicsObject"/> that stores arbitrary image data.
/// </summary>
public class Texture : GraphicsObject<Veldrid.Texture>, IBindableResource
{
    /// <summary>
    /// The default texture. A 1x1 white pixel.
    /// </summary>
    public static Texture Default => defaultLazy.Value;

    /// <summary>
    /// The width of this texture.
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// The height of this texture.
    /// </summary>
    /// <remarks>
    /// Only used if the texture kind is <see cref="TextureKind.Texture2D"/>.
    /// </remarks>
    public readonly int Height;

    /// <summary>
    /// The depth of this texture.
    /// </summary>
    /// <remarks>
    /// Only used if the texture kind is <see cref="TextureKind.Texture3D"/>.
    /// </remarks>
    public readonly int Depth;

    /// <summary>
    /// The number of array layers this texture has.
    /// </summary>
    public readonly int Layers;

    /// <summary>
    /// The number of mip map levels this texture has.
    /// </summary>
    public readonly int MipLevels;

    /// <summary>
    /// The kind of texture this is representing.
    /// </summary>
    public readonly TextureKind Kind;

    /// <summary>
    /// The format in which the image is stored in this texture.
    /// </summary>
    public readonly PixelFormat Format;

    /// <summary>
    /// The flags indicating how this texture will be used.
    /// </summary>
    public readonly TextureUsage Usage;

    /// <summary>
    /// The number of samples this image has.
    /// </summary>
    public readonly TextureSampleCount SampleCount;

    internal override Veldrid.Texture Resource { get; }

    /// <summary>
    /// Constructs a <see cref="TextureKind.Texture3D"/> texture.
    /// </summary>
    /// <remarks>
    /// <see cref="TextureKind.Texture3D"/> textures cannot have multiple array layers and thus the layers parameter is ignored.
    /// </remarks>
    public Texture(int width, int height, int depth, int mipLevels, int _, PixelFormat format, TextureUsage usage, TextureSampleCount sampleCount = TextureSampleCount.Count1)
        : this(width, height, depth, mipLevels, 1, format, usage, TextureKind.Texture3D, sampleCount)
    {
    }

    /// <summary>
    /// Constructs a <see cref="TextureKind.Texture2D"/> texture.
    /// </summary>
    public Texture(int width, int height, int mipLevels, int layers, PixelFormat format, TextureUsage usage, TextureSampleCount sampleCount = TextureSampleCount.Count1)
        : this(width, height, 1, mipLevels, layers, format, usage, TextureKind.Texture2D, sampleCount)
    {
    }

    /// <summary>
    /// Constructs a <see cref="TextureKind.Texture1D"/> texture.
    /// </summary>
    public Texture(int width, int mipLevels, int layers, PixelFormat format, TextureUsage usage, TextureSampleCount sampleCount = TextureSampleCount.Count1)
        : this(width, 1, 1, mipLevels, layers, format, usage, TextureKind.Texture1D, sampleCount)
    {
    }

    /// <summary>
    /// Constructs a <see cref="TextureKind.Texture2D"/> texture from a stream.
    /// </summary>
    public Texture(Stream stream, bool mipmaps = false, bool srgb = false)
        : this(Image.Load<Rgba32>(stream), mipmaps, srgb)
    {
    }

    internal Texture(Veldrid.Texture texture)
    {
        Kind = TextureKind.Texture2D;
        Width = (int)texture.Width;
        Depth = (int)texture.Depth;
        Usage = (TextureUsage)texture.Usage;
        Height = (int)texture.Height;
        Layers = (int)texture.ArrayLayers;
        Format = (PixelFormat)texture.Format;
        MipLevels = (int)texture.MipLevels;
        SampleCount = (TextureSampleCount)texture.SampleCount;
        Resource = texture;
    }

    private Texture(int width, int height, int depth, int mipLevels, int layers, PixelFormat format, TextureUsage usage, TextureKind kind, TextureSampleCount sampleCount)
    {
        Kind = kind;
        Width = width;
        Depth = depth;
        Usage = usage;
        Height = height;
        Layers = layers;
        Format = format;
        MipLevels = mipLevels;
        SampleCount = sampleCount;
        Resource = Context.Resources.CreateTexture(new((uint)Width, (uint)Height, (uint)Depth, (uint)MipLevels, (uint)Layers, Format.ToVeldrid(), Usage.ToVeldrid(), Kind.ToVeldrid(), SampleCount.ToVeldrid()));
    }

    private Texture(Image<Rgba32> image, bool mipmaps = false, bool srgb = false)
    {
        Kind = TextureKind.Texture2D;
        Usage = TextureUsage.Sampled;
        Depth = 1;
        Width = image.Width;
        Height = image.Height;
        Format = srgb ? PixelFormat.R8_G8_B8_A8_UNorm_SRgb : PixelFormat.R8_G8_B8_A8_UNorm;
        Layers = 1;
        MipLevels = mipmaps ? 1 + (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2)) : 1;
        Resource = Context.Resources.CreateTexture(new((uint)Width, (uint)Height, (uint)Depth, (uint)MipLevels, (uint)Layers, Format.ToVeldrid(), Usage.ToVeldrid(), Kind.ToVeldrid(), SampleCount.ToVeldrid()));
        uploadImagesToTexture(mipmaps ? generateMipmaps(image, MipLevels) : new[] { image });
    }

    public void SetData(nint data, int size, int x, int y, int z, int width, int height, int depth, int mipLevel, int layer)
    {
        Context.Device.UpdateTexture(Resource, data, (uint)size, (uint)x, (uint)y, (uint)z, (uint)width, (uint)height, (uint)depth, (uint)mipLevel, (uint)layer);
    }

    public unsafe void GetData(nint data, int subresource = 0)
    {
        using var staging = new Texture(Width, Height, Depth, MipLevels, Layers, Format, TextureUsage.Staging, SampleCount);
        using var commands = new CommandList();
        commands.Start();
        commands.CopyTexture(Resource, staging.Resource);
        commands.End();

        using var mapped = new TextureMappedResource(staging, MapMode.Read, subresource);
        Buffer.MemoryCopy((void*)mapped.Data, (void*)data, mapped.Size, mapped.Size);
    }

    private unsafe void uploadImagesToTexture(IEnumerable<Image<Rgba32>> images)
    {
        using var staging = new Texture(Width, Height, 1, MipLevels, Layers, Format, TextureUsage.Staging);
        using var commands = new CommandList();
        commands.Start();

        int level = 0;

        foreach (var image in images)
        {
            if (!image.DangerousTryGetSinglePixelMemory(out var memory))
                throw new InvalidOperationException(@"Failed to get pixel memory.");

            using var pinned = memory.Pin();
            using var mapped = new TextureMappedResource(staging, MapMode.Write, level);

            int rowWidth = image.Width * 4;

            if (rowWidth == mapped.RowPitch)
            {
                Unsafe.CopyBlock((void*)mapped.Data, pinned.Pointer, (uint)(image.Width * image.Height * 4));
            }
            else
            {
                for (int y = 0; y < image.Height; y++)
                {
                    byte* srcStart = (byte*)pinned.Pointer + (y * rowWidth);
                    byte* dstStart = (byte*)mapped.Data + (y * mapped.RowPitch);
                    Unsafe.CopyBlock(dstStart, srcStart, (uint)rowWidth);
                }
            }

            commands.CopyTexture(staging.Resource, 0, 0, 0, level, 0, Resource, 0, 0, 0, level, 0, image.Width, image.Height, 1, 1);

            level++;
        }

        commands.End();
    }

    private static readonly Lazy<Texture> defaultLazy = new(create_default_texture);
    private static Texture create_default_texture() => new(new Image<Rgba32>(1, 1, new Rgba32(255, 255, 255)));

    private static Image<Rgba32>[] generateMipmaps(Image<Rgba32> baseImage, int mipLevels)
    {
        int currentWidth = baseImage.Width;
        int currentHeight = baseImage.Height;

        var images = new Image<Rgba32>[mipLevels];
        images[0] = baseImage;

        int index = 1;
        while (currentWidth != 1 || currentHeight != 1)
        {
            int nextWidth = Math.Max(1, currentWidth / 2);
            int nextHeight = Math.Max(1, currentHeight / 2);

            images[index] = baseImage.Clone(ctx => ctx.Resize(nextWidth, nextHeight, KnownResamplers.Lanczos3));

            currentWidth = nextWidth;
            currentHeight = nextHeight;
            index++;
        }

        return images;
    }

    Veldrid.BindableResource IBindableResource.Resource => Resource;

    private class TextureMappedResource : MappedResource
    {
        /// <summary>
        /// The subresource mapped.
        /// </summary>
        public int SubResource => (int)Mapped.Subresource;

        /// <summary>
        /// Number of bytes between each depth slice of a <see cref="TextureKind.Texture3D"/> texture.
        /// </summary>
        public int DepthPitch => (int)Mapped.DepthPitch;

        /// <summary>
        /// The number of bytes in each row texels.
        /// </summary>
        public int RowPitch => (int)Mapped.RowPitch;

        internal TextureMappedResource(Texture texture, MapMode mode, int subresource = 0)
            : base(texture.Resource, mode, subresource)
        {
        }
    }
}
