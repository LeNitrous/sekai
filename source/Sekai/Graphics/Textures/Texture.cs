// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Sekai.Graphics.Textures;

/// <summary>
/// A storage object containing color data which can be used to draw on the screen.
/// </summary>
public class Texture : FrameworkObject
{
    /// <inheritdoc cref="INativeTexture.Width"/>
    public int Width => Native.Width;

    /// <inheritdoc cref="INativeTexture.Height"/>
    public int Height => Native.Height;

    /// <inheritdoc cref="INativeTexture.Depth"/>
    public int Depth => Native.Depth;


    /// <inheritdoc cref="INativeTexture.Levels"/>
    public int Level => Native.Levels;

    /// <inheritdoc cref="INativeTexture.Layers"/>
    public int Layers => Native.Layers;

    /// <inheritdoc cref="INativeTexture.Min"/>
    public FilterMode Min
    {
        get => Native.Min;
        set => Native.Min = value;
    }

    /// <inheritdoc cref="INativeTexture.Mag"/>
    public FilterMode Mag
    {
        get => Native.Mag;
        set => Native.Mag = value;
    }

    /// <inheritdoc cref="INativeTexture.WrapModeS"/>
    public WrapMode WrapModeS
    {
        get => Native.WrapModeS;
        set => Native.WrapModeS = value;
    }

    /// <inheritdoc cref="INativeTexture.WrapModeT"/>
    public WrapMode WrapModeT
    {
        get => Native.WrapModeT;
        set => Native.WrapModeT = value;
    }

    /// <inheritdoc cref="INativeTexture.WrapModeR"/>
    public WrapMode WrapModeR
    {
        get => Native.WrapModeR;
        set => Native.WrapModeR = value;
    }

    /// <inheritdoc cref="INativeTexture.Format"/>
    public PixelFormat Format => Native.Format;

    /// <inheritdoc cref="INativeTexture.Type"/>
    public TextureType Type => Native.Type;

    /// <inheritdoc cref="INativeTexture.Usage"/>
    public TextureUsage Usage => Native.Usage;

    /// <inheritdoc cref="INativeTexture.SampleCount"/>
    public TextureSampleCount SampleCount => Native.SampleCount;

    internal readonly INativeTexture Native;
    private readonly GraphicsContext context = Game.Resolve<GraphicsContext>();
    private readonly IGraphicsFactory factory = Game.Resolve<IGraphicsFactory>();

    private Texture(int width, int height, int depth, int level, int layers, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, TextureType type, TextureUsage usage, TextureSampleCount sampleCount, PixelFormat format)
    {
        Native = factory.CreateTexture(width, height, depth, level, layers, min, mag, wrapModeS, wrapModeT, wrapModeR, type, usage, sampleCount, format);
    }

    /// <inheritdoc cref="INativeTexture.Bind"/>
    public void Bind(int unit = 0)
    {
        context.BindTexture(this, unit);
    }

    /// <inheritdoc cref="INativeTexture.SetData"/>
    public void SetData(nint data, int x, int y, int z, int width, int height, int depth, int layer, int level)
    {
        if (x + width > Width || y + height > Height || z + depth > Depth || layer > Layers || level > Level)
            throw new InvalidOperationException(@"Attempting to set texture data beyond boundaries.");

        Native.SetData(data, x, y, z, width, height, depth, layer, level);
    }

    /// <inheritdoc cref="INativeTexture.SetData"/>
    public unsafe void SetData<T>(ReadOnlySpan<T> data, int x, int y, int z, int width, int height, int depth, int layer, int level)
        where T : unmanaged
    {
        fixed (T* ptr = data)
            SetData((nint)ptr, x, y, z, width, height, depth, layer, level);
    }

    /// <inheritdoc cref="INativeTexture.SetData"/>
    public void SetData<T>(T[] data, int x, int y, int z, int width, int height, int depth, int layer, int level)
        where T : unmanaged
    {
        SetData<T>(data.AsSpan(), x, y, z, width, height, depth, layer, level);
    }

    protected override void Destroy()
    {
        Native.Dispose();
    }

    /// <summary>
    /// Creates a new one-dimensional texture.
    /// </summary>
    /// <param name="width">The texture width.</param>
    /// <param name="format">The pixel format of the texture.</param>
    /// <param name="levels">The mip levels of the texture.</param>
    /// <param name="layers">The number of layers of the texture.</param>
    /// <param name="min">The filter strategy when minifying the texture.</param>
    /// <param name="mag">The filter strategy when magnifying the texture.</param>
    /// <param name="wrapMode">The wrap mode for the texture.</param>
    /// <param name="usage">The nature of usage for the texture.</param>
    public static Texture New1D(int width, PixelFormat format, int levels = 1, int layers = 1, FilterMode min = FilterMode.Linear, FilterMode mag = FilterMode.Linear, WrapMode wrapMode = WrapMode.None, TextureUsage usage = TextureUsage.Sampled)
    {
        return new Texture(width, 1, 1, levels, layers, min, mag, wrapMode, WrapMode.None, WrapMode.None, TextureType.Texture1D, usage, TextureSampleCount.Count1, format);
    }

    /// <summary>
    /// Creates a new two-dimensional texture.
    /// </summary>
    /// <param name="width">The texture width.</param>
    /// <param name="height">The texture height.</param>
    /// <param name="format">The pixel format of the texture.</param>
    /// <param name="levels">The mip levels of the texture.</param>
    /// <param name="layers">The number of layers of the texture.</param>
    /// <param name="min">The filter strategy when minifying the texture.</param>
    /// <param name="mag">The filter strategy when magnifying the texture.</param>
    /// <param name="wrapModeS">The wrap mode for the texture in the X axis.</param>
    /// <param name="wrapModeT">The wrap mode for the texture in the Y axis</param>
    /// <param name="usage">The nature of usage for the texture.</param>
    /// <param name="sampleCount">The texture sample count.</param>
    public static Texture New2D(int width, int height, PixelFormat format, int levels = 1, int layers = 1, FilterMode min = FilterMode.Linear, FilterMode mag = FilterMode.Linear, WrapMode wrapModeS = WrapMode.None, WrapMode wrapModeT = WrapMode.None, TextureUsage usage = TextureUsage.Sampled, TextureSampleCount sampleCount = TextureSampleCount.Count1)
    {
        return new Texture(width, height, 1, levels, layers, min, mag, wrapModeS, wrapModeT, WrapMode.None, TextureType.Texture2D, usage, sampleCount, format);
    }

    /// <summary>
    /// Creates a new three-dimensional texture.
    /// </summary>
    /// <param name="width">The texture width.</param>
    /// <param name="height">The texture height.</param>
    /// <param name="depth">The texture depth.</param>
    /// <param name="format">The pixel format of the texture.</param>
    /// <param name="levels">The mip levels of the texture.</param>
    /// <param name="layers">The number of layers of the texture.</param>
    /// <param name="min">The filter strategy when minifying the texture.</param>
    /// <param name="mag">The filter strategy when magnifying the texture.</param>
    /// <param name="wrapModeS">The wrap mode for the texture in the X axis.</param>
    /// <param name="wrapModeT">The wrap mode for the texture in the Y axis</param>
    /// <param name="wrapModeR">The wrap mode for the texture in the Z axis.</param>
    /// <param name="usage">The nature of usage for the texture.</param>
    public static Texture New3D(int width, int height, int depth, PixelFormat format, int levels = 1, FilterMode min = FilterMode.Linear, FilterMode mag = FilterMode.Linear, WrapMode wrapModeS = WrapMode.None, WrapMode wrapModeT = WrapMode.None, WrapMode wrapModeR = WrapMode.None, TextureUsage usage = TextureUsage.Sampled)
    {
        return new Texture(width, height, depth, levels, 1, min, mag, wrapModeS, wrapModeT, wrapModeR, TextureType.Texture3D, usage, TextureSampleCount.Count1, format);
    }

    /// <summary>
    /// Loads a two-dimensional texture from an image.
    /// </summary>
    /// <param name="stream">The image data as a stream.</param>
    public static Texture Load(Stream stream)
    {
        return Load(Image.Load<Rgba32>(stream));
    }

    /// <summary>
    /// Loads a two-dimensional texture from bytes.
    /// </summary>
    /// <param name="data">The image data in a byte array.</param>
    public static Texture Load(byte[] data)
    {
        return Load(Image.Load<Rgba32>(data));
    }

    /// <summary>
    /// Loads a two-dimensional texture from bytes.
    /// </summary>
    /// <param name="data">The image data in a byte span.</param>
    public static Texture Load(ReadOnlySpan<byte> data)
    {
        return Load(Image.Load<Rgba32>(data));
    }

    /// <summary>
    /// Loads a two-dimensional texture from an image.
    /// </summary>
    /// <param name="image">The image data.</param>
    public static unsafe Texture Load(Image<Rgba32> image)
    {
        var texture = New2D(image.Width, image.Height, PixelFormat.R8_G8_B8_A8_UNorm_SRgb);

        Span<byte> data = stackalloc byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(data);

        fixed (byte* ptr = data)
            texture.SetData((nint)ptr, 0, 0, 0, image.Width, image.Height, 1, 0, 0);

        return texture;
    }
}