// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Sekai.Allocation;

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

    internal readonly NativeBuffer Native;

    public Buffer(int capacity, bool dynamic = false)
    {
        if (capacity <= 0)
            throw new ArgumentException(@"Capacity cannot be a zero or negative value.");

        Native = Graphics.CreateBuffer(capacity, dynamic);
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

    protected sealed override void DestroyGraphics() => Native.Dispose();
}

/// <summary>
/// A storage object capable of storing arbitrary contents in the GPU.
/// </summary>
public class Buffer<T> : Buffer
    where T : unmanaged
{
    /// <summary>
    /// THe number of elements this buffer contains.
    /// </summary>
    public readonly int Count;

    public Buffer(int count, bool dynamic = false)
        : base(count * Unsafe.SizeOf<T>(), dynamic)
    {
        Count = count;
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
}
