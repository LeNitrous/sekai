// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sekai.Graphics.Vertices;

namespace Sekai.Graphics.Buffers;

/// <summary>
/// A storage object capable of storing arbitrary contents in the GPU.
/// </summary>
public class Buffer : GraphicsObject
{
    /// <summary>
    /// The number in bytes of how much this buffer can store.
    /// </summary>
    public int Capacity => Native.Capacity;

    internal readonly INativeBuffer Native;

    public Buffer(int capacity, bool dynamic = false)
    {
        if (capacity <= 0)
            throw new ArgumentException(@"Capacity cannot be a zero or negative value.");

        Native = Context.Factory.CreateBuffer(capacity, dynamic);
    }

    /// <summary>
    /// Sets the data in the buffer.
    /// </summary>
    /// <param name="data">The data to store.</param>
    /// <param name="size">The size of the data being stored.</param>
    /// <param name="offset">The offset in the buffer to store the data.</param>
    /// <exception cref="IndexOutOfRangeException">Data content exceeds buffer capacity.</exception>
    public void SetData(nint data, int size, int offset = 0)
    {
        if (offset + size > Capacity)
            throw new IndexOutOfRangeException(@"Region of memory to set data exceeds the buffer's capacity.");

        Native.SetData(data, size, offset);
    }

    /// <summary>
    /// Gets the data in the buffer.
    /// </summary>
    /// <param name="dest">The destination where data will be copied to.</param>
    /// <param name="size">The number of bytes to copy.</param>
    /// <param name="offset">The offset in the buffer to retrieve the data.</param>
    /// <exception cref="IndexOutOfRangeException">Destination exceeds buffer capacity.</exception>
    public void GetData(nint dest, int size, int offset = 0)
    {
        if (offset + size > Capacity)
            throw new IndexOutOfRangeException(@"Region of memory to set data exceeds the buffer's capacity.");

        Native.GetData(dest, size, offset);
    }

    /// <summary>
    /// Binds this buffer as an index buffer.
    /// </summary>
    /// <param name="format">The index format used.</param>
    public void Bind(IndexFormat format)
    {
        Context.BindIndexBuffer(this, format);
    }

    /// <summary>
    /// Binds this buffer as a vertex buffer.
    /// </summary>
    /// <param name="layout">The layout used to define this buffer.</param>
    public void Bind(IVertexLayout layout)
    {
        Context.BindVertexBuffer(this, layout);
    }

    protected sealed override void Destroy() => Native.Dispose();
}

/// <summary>
/// A storage object capable of storing arbitrary contents in the GPU.
/// </summary>
public class Buffer<T> : Buffer
    where T : unmanaged
{
    public Buffer(int count, bool dynamic = false)
        : base(count * Unsafe.SizeOf<T>(), dynamic)
    {
    }

    /// <summary>
    /// Binds this buffer with its usage determined by its type.
    /// </summary>
    public void Bind()
    {
        if (is_vertex_buffer)
        {
            Bind(layout.Value);
        }
        else if (supported_index_formats.TryGetValue(typeof(T), out var format))
        {
            Bind(format);
        }
        else
        {
            throw new NotSupportedException(@"Unable to determine type of buffer for binding.");
        }
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int, int)"/>
    public void SetData(T data, int offset = 0)
    {
        SetData(ref data, offset);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int, int)"/>
    public unsafe void SetData(ref T data, int offset = 0)
    {
        fixed (T* ptr = &data)
            SetData((nint)ptr, Unsafe.SizeOf<T>(), offset);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int, int)"/>
    public unsafe void SetData(ReadOnlySpan<T> data, int offset = 0)
    {
        fixed (T* ptr = data)
            SetData((nint)ptr, Unsafe.SizeOf<T>() * data.Length, offset);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int, int)"/>
    public void SetData(T[] data, int offset = 0)
    {
        SetData(data.AsSpan(), offset);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int, int)"/>
    public T GetData(int offset = 0)
    {
        T dest = default;
        GetData(ref dest, offset);
        return dest;
    }

    /// <inheritdoc cref="Buffer.GetData(nint, int, int)"/>
    public unsafe void GetData(ref T dest, int offset = 0)
    {
        fixed (T* ptr = &dest)
            GetData((nint)ptr, Unsafe.SizeOf<T>(), offset);
    }

    /// <inheritdoc cref="Buffer.GetData(nint, int, int)"/>
    public unsafe void GetData(ReadOnlySpan<T> dest, int offset = 0)
    {
        fixed (T* ptr = dest)
            GetData((nint)ptr, Unsafe.SizeOf<T>() * dest.Length, offset);
    }

    /// <inheritdoc cref="Buffer.GetData(nint, int, int)"/>
    public void GetData(T[] dest, int offset = 0)
    {
        GetData(dest.AsSpan(), offset);
    }

    private static readonly Dictionary<Type, IndexFormat> supported_index_formats = new()
    {
        { typeof(int), IndexFormat.UInt32 },
        { typeof(uint), IndexFormat.UInt32 },
        { typeof(short), IndexFormat.UInt16 },
        { typeof(ushort), IndexFormat.UInt16 },
    };

    private static readonly bool is_vertex_buffer = typeof(T).IsAssignableTo(typeof(IVertex));
    private static readonly Lazy<IVertexLayout> layout = new(() => VertexLayout.From(typeof(T)));
}
