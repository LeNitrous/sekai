// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Graphics;

public class IndexBuffer : BindableBuffer
{
    public readonly int Count;
    public readonly IndexBufferFormat Format;

    public IndexBuffer(IGraphicsDevice device, int count, IndexBufferFormat format)
        : base(device, getFormatStride(format) * count, BufferUsage.Index)
    {
        Count = count;
        Format = format;
    }

    public override void Bind(ICommandQueue queue)
    {
        queue.SetIndexBuffer(Buffer, Format);
    }

    private static int getFormatStride(IndexBufferFormat format)
    {
        return format switch
        {
            IndexBufferFormat.UInt16 => 2,
            IndexBufferFormat.UInt32 => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };
    }
}

public class IndexBuffer<T> : IndexBuffer
    where T : unmanaged, IEquatable<T>
{
    public IndexBuffer(IGraphicsDevice device, int count)
        : base(device, count, getIndexFormat(typeof(T)))
    {
    }

    public IndexBuffer(IGraphicsDevice device, T[] data)
        : this(device, data.Length)
    {
        SetData(data);
    }

    public IndexBuffer(IGraphicsDevice device, Span<T> data)
        : this(device, data.Length)
    {
        SetData(data);
    }

    /// <summary>
    /// Sets the data for this index buffer.
    /// </summary>
    public void SetData(T data, int offset = 0)
    {
        Device.UpdateBufferData(Buffer, ref data, (uint)offset);
    }

    /// <summary>
    /// Sets the data for this index buffer.
    /// </summary>
    public void SetData(ref T data, int offset = 0)
    {
        Device.UpdateBufferData(Buffer, ref data, (uint)offset);
    }

    /// <summary>
    /// Sets the data for this index buffer.
    /// </summary>
    public void SetData(T[] data)
    {
        Device.UpdateBufferData(Buffer, data);
    }

    /// <summary>
    /// Sets the data for this index buffer.
    /// </summary>
    public void SetData(Span<T> data)
    {
        Device.UpdateBufferData(Buffer, data);
    }

    private static IndexBufferFormat getIndexFormat(Type type)
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            return IndexBufferFormat.UInt32;
        }

        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
        {
            return IndexBufferFormat.UInt16;
        }

        throw new ArgumentException($"{type} is not a supported index format.");
    }
}
