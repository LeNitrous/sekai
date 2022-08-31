// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct BufferDescription : IEquatable<BufferDescription>
{
    /// <summary>
    /// The size of the <see cref="IBuffer"/> in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// Determines how this <see cref="IBuffer"/> will be used.
    /// </summary>
    public BufferUsage Usage;

    /// <summary>
    /// For structured buffers, this determines the size in bytes of a single structure.
    /// For other buffer types, this value must be zero.
    /// </summary>
    public uint StructureByteStride;

    public BufferDescription(uint size, BufferUsage usage)
    {
        Size = size;
        Usage = usage;
        StructureByteStride = 0;
    }

    public BufferDescription(uint size, BufferUsage usage, uint structureByteStride)
    {
        Size = size;
        Usage = usage;
        StructureByteStride = structureByteStride;
    }

    public override bool Equals(object? obj)
    {
        return obj is BufferDescription description && Equals(description);

    }

    public bool Equals(BufferDescription other)
    {
        return Size == other.Size &&
               Usage == other.Usage &&
               StructureByteStride == other.StructureByteStride;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Size, Usage, StructureByteStride);
    }

    public static bool operator ==(BufferDescription left, BufferDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BufferDescription left, BufferDescription right)
    {
        return !(left == right);
    }
}
