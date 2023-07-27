// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using Sekai.Graphics;

namespace Sekai.Headless;

internal sealed class HeadlessTexture : Texture
{
    public override TextureType Type { get; }

    public override int Width { get; }

    public override int Height { get; }

    public override int Depth { get; }

    public override PixelFormat Format { get; }

    public override int Levels { get; }

    public override int Layers { get; }

    public override TextureUsage Usage { get; }

    public override TextureSampleCount Count { get; }

    private bool isDisposed;
    private readonly IMemoryOwner<byte> owner;

    public HeadlessTexture(TextureDescription description)
    {
        Type = description.Type;
        Width = description.Width;
        Height = description.Height;
        Depth = description.Depth;
        Format = description.Format;
        Levels = description.Levels;
        Layers = description.Layers;
        Usage = description.Usage;
        Count = description.Count;
        owner = MemoryPool<byte>.Shared.Rent(Width * Height * Depth * Format.SizeOfFormat());
    }

    public override unsafe void SetData(nint data, uint size, int level, int layer, int x, int y, int z, int width, int height, int depth)
    {
        Span<byte> src = new(data.ToPointer(), (int)size);
        src.CopyTo(getOffsetSpan(x, y, z, width, height, depth));
    }

    public override unsafe void GetData(nint data, uint size, int level, int layer, int x, int y, int z, int width, int height, int depth)
    {
        Span<byte> dst = new(data.ToPointer(), (int)size);
        getOffsetSpan(x, y, z, width, height, depth).CopyTo(dst);
    }

    private Span<byte> getOffsetSpan(int x, int y, int z, int width, int height, int depth)
    {
        int region = width * height * depth * Format.SizeOfFormat();
        int offset = (z * Width * Height) + (y * Width) + x;
        return owner.Memory.Span[offset..(offset + region)];
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        owner.Dispose();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
