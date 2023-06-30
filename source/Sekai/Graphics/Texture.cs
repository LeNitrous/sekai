// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;

namespace Sekai.Graphics;

public abstract class Texture : IDisposable
{
    /// <inheritdoc cref="TextureDescription.Type"/>
    public abstract TextureType Type { get; }

    /// <inheritdoc cref="TextureDescription.Width"/>
    public abstract int Width { get; }

    /// <inheritdoc cref="TextureDescription.Height"/>
    public abstract int Height { get; }

    /// <inheritdoc cref="TextureDescription.Depth"/>
    public abstract int Depth { get; }

    /// <inheritdoc cref="TextureDescription.Format"/>
    public abstract PixelFormat Format { get; }

    /// <inheritdoc cref="TextureDescription.Levels"/>
    public abstract int Levels { get; }

    /// <inheritdoc cref="TextureDescription.Layers"/>
    public abstract int Layers { get; }

    /// <inheritdoc cref="TextureDescription.Usage"/>
    public abstract TextureUsage Usage { get; }

    /// <inheritdoc cref="TextureDescription.Count"/>
    public abstract TextureSampleCount Count { get; }

    /// <summary>
    /// Sets a region of a texture.
    /// </summary>
    /// <param name="data">The data to set.</param>
    /// <param name="size">The size of the data to be uploaded.</param>
    /// <param name="level">The mip level where to update the texture.</param>
    /// <param name="layer">The array layer where to update the texture.</param>
    /// <param name="x">The x offset of the region to update.</param>
    /// <param name="y">The y offset of the region to update.</param>
    /// <param name="z">The z offset of the region to update.</param>
    /// <param name="width">The width of region to update.</param>
    /// <param name="height">The height region to update.</param>
    /// <param name="depth">The depth of region to update.</param>
    public abstract void SetData(nint data, uint size, int level, int layer, int x, int y, int z, int width, int height, int depth);

    /// <inheritdoc cref="SetData(nint, uint, int, int, int, int, int, int, int, int)"/>
    public void SetData<T>(T[] data, int level, int layer, int x, int y, int z, int width, int height, int depth)
        where T : unmanaged
    {
        SetData((ReadOnlySpan<T>)data.AsSpan(), level, layer, x, y, z, width, height, depth);
    }

    /// <inheritdoc cref="SetData(nint, uint, int, int, int, int, int, int, int, int)"/>
    public unsafe void SetData<T>(ReadOnlySpan<T> data, int level, int layer, int x, int y, int z, int width, int height, int depth)
        where T : unmanaged
    {
        fixed (void* handle = data)
            SetData((nint)handle, (uint)(Unsafe.SizeOf<T>() * data.Length), level, layer, x, y, z, width, height, depth);
    }

    /// <summary>
    /// Gets a region of a texture.
    /// </summary>
    /// <param name="data">The location where to store the retrieved data.</param>
    /// <param name="size">The size of the location to store data to.</param>
    /// <param name="level">The mip level where to get the data.</param>
    /// <param name="layer">The array layer where to get the data.</param>
    /// <param name="x">The x offset of the region to get the data.</param>
    /// <param name="y">The y offset of the region to get the data.</param>
    /// <param name="z">The z offset of the region to get the data.</param>
    /// <param name="width">The width of region to get the data.</param>
    /// <param name="height">The height region to get the data.</param>
    /// <param name="depth">The depth of region to get the data.</param>
    public abstract void GetData(nint data, uint size, int level, int layer, int x, int y, int z, int width, int height, int depth);

    /// <inheritdoc cref="GetData(nint, uint, int, int, int, int, int, int, int, int)"/>
    public void GetData<T>(T[] data, int level, int layer, int x, int y, int z, int width, int height, int depth)
        where T : unmanaged
    {
        GetData(data.AsSpan(), level, layer, x, y, z, width, height, depth);
    }

    /// <inheritdoc cref="GetData(nint, uint, int, int, int, int, int, int, int, int)"/>
    public unsafe void GetData<T>(Span<T> data, int level, int layer, int x, int y, int z, int width, int height, int depth)
        where T : unmanaged
    {
        fixed (void* handle = data)
            GetData((nint)handle, (uint)(Unsafe.SizeOf<T>() * data.Length), level, layer, x, y, z, width, height, depth);
    }

    public abstract void Dispose();
}
