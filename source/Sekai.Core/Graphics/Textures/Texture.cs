// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Sekai.Extensions;

namespace Sekai.Graphics.Textures;

/// <summary>
/// Represents a graphics object that contains image data.
/// </summary>
public sealed class Texture : IDisposable
{
    /// <summary>
    /// The texture's width.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The texture's depth.
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// The texture's height.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The texture's mip levels.
    /// </summary>
    public int Levels { get; }

    /// <summary>
    /// The texture's array layers.
    /// </summary>
    public int Layers { get; }

    /// <summary>
    /// The texture's flags.
    /// </summary>
    public TextureFlag Flag { get; }

    /// <summary>
    /// The texture's kind.
    /// </summary>
    public TextureKind Kind { get; }

    /// <summary>
    /// The texture's pixel format.
    /// </summary>
    public PixelFormat Format { get; }

    private bool isDisposed;
    private readonly Veldrid.Texture texture;

    private Texture(Veldrid.Texture texture, int width, int height, int depth, int levels, int layers, TextureFlag flag, TextureKind kind, PixelFormat format)
    {
        Flag = flag;
        Kind = kind;
        Width = width;
        Depth = depth;
        Height = height;
        Levels = levels;
        Layers = layers;
        Format = format;
        this.texture = texture;
    }

    /// <summary>
    /// Sets the texture's data at a given region.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The pointer to the data to be set.</param>
    /// <param name="size">The size of the pointer.</param>
    /// <param name="x">The x-offset where overwriting data will begin.</param>
    /// <param name="y">The y-offset where overwriting data will begin.</param>
    /// <param name="z">The z-offset where overwriting data will begin.</param>
    /// <param name="width">The width of the texture region to be overwritten.</param>
    /// <param name="height">The height of the texture region to be overwritten.</param>
    /// <param name="depth">The depth of the texture region to be overwritten.</param>
    /// <param name="level">The texture mip level where data will be overwritten.</param>
    /// <param name="layer"><The texture array layer where data will be overwritten./param>
    public void SetData(GraphicsDevice device, nint data, int size, int x, int y, int z, int width, int height, int depth, int level, int layer)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(Texture));
        }

        if (Width < x + width || Height < y + height || Depth < z + depth || Levels < level || Layers < layer)
        {
            throw new IndexOutOfRangeException("Attempted to update texture out of bounds.");
        }

        if (size <= 0)
        {
            throw new ArgumentException("Invalid source size.", nameof(size));
        }

        if (data == IntPtr.Zero)
        {
            throw new NullReferenceException("Source is null.");
        }

        using var staging = getStagingTexture(device);
        using var command = device.Factory.CreateCommandList();

        device.Veldrid.UpdateTexture(staging, data, (uint)size, (uint)x, (uint)y, (uint)z, (uint)width, (uint)height, (uint)depth, (uint)level, (uint)layer);

        command.Begin();
        command.CopyTexture(staging, texture);
        command.End();

        device.Veldrid.SubmitCommands(command);
        device.Veldrid.WaitForIdle();
    }

    /// <summary>
    /// Sets the texture's data at a given region.
    /// </summary>
    /// <typeparam name="T">The data's type.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The read-only span of data to set.</param>
    /// <param name="x">The x-offset where overwriting data will begin.</param>
    /// <param name="y">The y-offset where overwriting data will begin.</param>
    /// <param name="z">The z-offset where overwriting data will begin.</param>
    /// <param name="width">The width of the texture region to be overwritten.</param>
    /// <param name="height">The height of the texture region to be overwritten.</param>
    /// <param name="depth">The depth of the texture region to be overwritten.</param>
    /// <param name="level">The texture mip level where data will be overwritten.</param>
    /// <param name="layer"><The texture array layer where data will be overwritten./param>
    public unsafe void SetData<T>(GraphicsDevice device, ReadOnlySpan<T> data, int x, int y, int z, int width, int height, int depth, int level, int layer)
        where T : unmanaged
    {
        fixed (void* ptr = data)
        {
            SetData(device, (nint)ptr, Unsafe.SizeOf<T>() * data.Length, x, y, z, width, height, depth, level, layer);
        }
    }

    /// <summary>
    /// Sets the texture's data at a given region.
    /// </summary>
    /// <typeparam name="T">The data's type.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The array of data to set.</param>
    /// <param name="x">The x-offset where overwriting data will begin.</param>
    /// <param name="y">The y-offset where overwriting data will begin.</param>
    /// <param name="z">The z-offset where overwriting data will begin.</param>
    /// <param name="width">The width of the texture region to be overwritten.</param>
    /// <param name="height">The height of the texture region to be overwritten.</param>
    /// <param name="depth">The depth of the texture region to be overwritten.</param>
    /// <param name="level">The texture mip level where data will be overwritten.</param>
    /// <param name="layer"><The texture array layer where data will be overwritten./param>
    public void SetData<T>(GraphicsDevice device, T[] data, int x, int y, int z, int width, int height, int depth, int level, int layer)
        where T : unmanaged
    {
        SetData(device, (ReadOnlySpan<T>)data.AsSpan(), x, y, z, width, height, depth, level, layer);
    }

    /// <summary>
    /// Sets the texture's data at a given region.
    /// </summary>
    /// <typeparam name="T">The data's type.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="x">The x-offset where overwriting data will begin.</param>
    /// <param name="y">The y-offset where overwriting data will begin.</param>
    /// <param name="z">The z-offset where overwriting data will begin.</param>
    /// <param name="width">The width of the texture region to be overwritten.</param>
    /// <param name="height">The height of the texture region to be overwritten.</param>
    /// <param name="depth">The depth of the texture region to be overwritten.</param>
    /// <param name="level">The texture mip level where data will be overwritten.</param>
    /// <param name="layer"><The texture array layer where data will be overwritten./param>
    public unsafe void SetData<T>(GraphicsDevice device, ref T data, int x, int y, int z, int width, int height, int depth, int level, int layer)
        where T : unmanaged
    {
        SetData(device, (nint)Unsafe.AsPointer(ref data), Unsafe.SizeOf<T>(), x, y, z, width, height, depth, level, layer);
    }

    /// <summary>
    /// Sets the texture's data at a given region.
    /// </summary>
    /// <typeparam name="T">The data's type.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="x">The x-offset where overwriting data will begin.</param>
    /// <param name="y">The y-offset where overwriting data will begin.</param>
    /// <param name="z">The z-offset where overwriting data will begin.</param>
    /// <param name="width">The width of the texture region to be overwritten.</param>
    /// <param name="height">The height of the texture region to be overwritten.</param>
    /// <param name="depth">The depth of the texture region to be overwritten.</param>
    /// <param name="level">The texture mip level where data will be overwritten.</param>
    /// <param name="layer"><The texture array layer where data will be overwritten./param>
    public unsafe void SetData<T>(GraphicsDevice device, T data, int x, int y, int z, int width, int height, int depth, int level, int layer)
        where T : unmanaged
    {
        SetData(device, ref data, x, y, z, width, height, depth, level, layer);
    }

    /// <summary>
    /// Gets the texture data and stores it in a pointer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The pointer where retrieved data will be stored.</param>
    /// <param name="size">The size in bytes to store the data.</param>
    /// <param name="level">The mip level where data will be retrieved.</param>
    /// <param name="layer">The array layer where data will be retrieved</param>
    public unsafe void GetData(GraphicsDevice device, nint data, int size, int level, int layer)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(Texture));
        }

        if (level > Levels || layer > Layers || level < 0 || layer < 0 || size <= 0)
        {
            throw new ArgumentException("Attempted to read data out of bounds.");
        }

        using var staging = getStagingTexture(device);
        using var command = device.Factory.CreateCommandList();

        command.Begin();
        command.CopyTexture(texture, staging);
        command.End();

        device.Veldrid.SubmitCommands(command);
        device.Veldrid.WaitForIdle();

        uint subresource = texture.CalculateSubresource((uint)level, (uint)layer);

        var mapped = device.Veldrid.Map(staging, Veldrid.MapMode.Read, subresource);
        Unsafe.CopyBlock(data.ToPointer(), mapped.Data.ToPointer(), Math.Min((uint)size, mapped.SizeInBytes));

        device.Veldrid.Unmap(staging, subresource);
    }

    /// <summary>
    /// Gets the texture data as a span of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast the data as.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The span to store the retrieved texture data.</param>
    /// <param name="level">The mip level where data will be retrieved.</param>
    /// <param name="layer">The array layer where data will be retrieved.</param>
    public unsafe void GetData<T>(GraphicsDevice device, Span<T> data, int level, int layer)
        where T : unmanaged
    {
        fixed (void* ptr = data)
        {
            GetData(device, (nint)ptr, Unsafe.SizeOf<T>() * data.Length, level, layer);
        }
    }

    /// <summary>
    /// Gets the texture data as an array of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast the data as.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The array to store the retrieved texture data.</param>
    /// <param name="level">The mip level where data will be retrieved.</param>
    /// <param name="layer">The array layer where data will be retrieved.</param>
    public void GetData<T>(GraphicsDevice device, T[] data, int level, int layer)
        where T : unmanaged
    {
        GetData(device, data.AsSpan(), level, layer);
    }

    /// <summary>
    /// Gets the texture data as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast the data as.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The texture data as <typeparamref name="T"/></param>
    /// <param name="level">The mip level where data will be retrieved.</param>
    /// <param name="layer">The array layer where data will be retrieved.</param>
    public unsafe void GetData<T>(GraphicsDevice device, ref T data, int level, int layer)
        where T : unmanaged
    {
        GetData(device, (nint)Unsafe.AsPointer(ref data), Unsafe.SizeOf<T>(), level, layer);
    }

    /// <summary>
    /// Gets the texture data as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast the data as.</typeparam>
    /// <param name="device">The graphics device.</param>
    /// <param name="level">The mip level where data will be retrieved.</param>
    /// <param name="layer">The array layer where data will be retrieved.</param>
    /// <returns>The texture data as <typeparamref name="T"/>.</returns>
    public T GetData<T>(GraphicsDevice device, int level, int layer)
        where T : unmanaged
    {
        T data = default;
        GetData(device, ref data, level, layer);
        return data;
    }

    private Veldrid.Texture getStagingTexture(GraphicsDevice device)
    {
        return device.Factory.CreateTexture(new(texture.Width, texture.Height, texture.Depth, texture.MipLevels, texture.ArrayLayers, texture.Format, Veldrid.TextureUsage.Staging, texture.Type));
    }

    /// <summary>
    /// Create a one-dimensional texture.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="width">The texture width.</param>
    /// <param name="format">The texture format.</param>
    /// <param name="usage">The texture usage.</param>
    /// <param name="levels">The texture mip level count.</param>
    /// <param name="layers">The texture array layer count.</param>
    /// <returns>A one-dimensional texture.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid width has been passed.</exception>
    public static Texture Create(GraphicsDevice device, int width, PixelFormat format = PixelFormat.R8G8B8A8_UNorm_SRgb, TextureFlag usage = TextureFlag.Sampled, int levels = 1, int layers = 1)
    {
        if (width <= 0)
        {
            throw new ArgumentException("Invalid texture width.", nameof(width));
        }

        return create(device, width, 1, 1, format, usage, TextureKind.Texture1D, levels, layers);
    }

    /// <summary>
    /// Creates a two-dimensional texture.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="width">The texture width.</param>
    /// <param name="height">The texture height</param>
    /// <param name="format">The texture format.</param>
    /// <param name="usage">The texture usage.</param>
    /// <param name="levels">The texture mip level count.</param>
    /// <param name="layers">The texture array layer count.</param>
    /// <returns>A one-dimensional texture.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid width or height has been passed.</exception>
    public static Texture Create(GraphicsDevice device, int width, int height, PixelFormat format = PixelFormat.R8G8B8A8_UNorm_SRgb, TextureFlag usage = TextureFlag.Sampled, int levels = 1, int layers = 1)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Invalid texture size.");
        }

        return create(device, width, height, 1, format, usage, TextureKind.Texture2D, levels, layers);
    }

    /// <summary>
    /// Creates a three-dimensional texture.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="width">The texture width.</param>
    /// <param name="height">The texture height</param>
    /// <param name="depth">The texture depth.</param>
    /// <param name="format">The texture format.</param>
    /// <param name="usage">The texture usage.</param>
    /// <param name="levels">The texture mip level count.</param>
    /// <param name="layers">The texture array layer count.</param>
    /// <returns>A one-dimensional texture.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid width or height has been passed.</exception>
    public static Texture Create(GraphicsDevice device, int width, int height, int depth, PixelFormat format = PixelFormat.R8G8B8A8_UNorm_SRgb, TextureFlag usage = TextureFlag.Sampled, int levels = 1, int layers = 1)
    {
        if (width <= 0 || height <= 0 || depth <= 0)
        {
            throw new ArgumentException("Invalid texture size.");
        }

        return create(device, width, height, depth, format, usage, TextureKind.Texture3D, levels, layers);
    }

    /// <summary>
    /// Creates a framebuffer attachment for this texture.
    /// </summary>
    /// <param name="level">The mip level to expose to the framebuffer.</param>
    /// <param name="layer">The array layer to expose to the framebuffer.</param>
    /// <returns>A framebuffer attachment descriptor.</returns>
    internal Veldrid.FramebufferAttachmentDescription ToFramebufferAttachment(int level = 0, int layer = 0) => new(texture, (uint)layer, (uint)level);

    private static Texture create(GraphicsDevice device, int width, int height, int depth, PixelFormat format, TextureFlag flag, TextureKind kind, int levels, int layers)
    {
        if (format.IsDepthStencil())
        {
            throw new ArgumentException($"{format} is not a valid color format.", nameof(format));
        }

        if (levels <= 0)
        {
            throw new ArgumentException("Invalid texture mipmap levels.", nameof(levels));
        }

        if (layers <= 0)
        {
            throw new ArgumentException("Invalid texture array layer count.", nameof(layers));
        }

        var texture = device.Factory.CreateTexture(new
        (
            (uint)width,
            (uint)height,
            (uint)depth,
            (uint)levels,
            (uint)layers,
            format.AsVeldridFormat(),
            flag.AsVeldridUsage(),
            kind.AsVeldridType(),
            Veldrid.TextureSampleCount.Count1
        ));

        return new(texture, width, height, depth, levels, layers, flag, kind, format);
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
    }
}
