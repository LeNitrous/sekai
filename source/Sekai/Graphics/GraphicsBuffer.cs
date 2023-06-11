// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sekai.Graphics;

/// <summary>
/// A buffer that stores data on the GPU.
/// </summary>
public abstract class GraphicsBuffer : IDisposable
{
    /// <summary>
    /// The buffer's capacity in bytes.
    /// </summary>
    public abstract int Capacity { get; }

    /// <summary>
    /// The buffer's type.
    /// </summary>
    public abstract BufferType Type { get; }

    /// <summary>
    /// Whether this buffer is often written or read to or not.
    /// </summary>
    public abstract bool Dynamic { get; }

    /// <summary>
    /// Maps the buffer making its contents visible in the CPU-accessible address space.
    /// </summary>
    /// <param name="mode">The mode of mapping.</param>
    /// <returns>The pointer to the contents of the buffer.</returns>
    public abstract nint Map(MapMode mode);

    /// <summary>
    /// Unmaps the buffer and submits it to the GPU.
    /// </summary>
    public abstract void Unmap();

    /// <summary>
    /// Sets the data to the buffer.
    /// </summary>
    /// <param name="data">The data to set the buffer with.</param>
    /// <param name="size">The size in bytes of the data.</param>
    /// <param name="offset">The offset where to update the buffer.</param>
    public abstract void SetData(nint data, uint size, uint offset = 0);

    /// <inheritdoc cref="SetData(nint, uint, uint)"/>
    /// <typeparam name="T">The type of data to set the buffer to.</typeparam>
    public void SetData<T>(T data, uint offset = 0)
        where T : unmanaged
    {
        SetData(MemoryMarshal.CreateReadOnlySpan(ref data, 1), offset);
    }

    /// <inheritdoc cref="SetData{T}(T, uint)"/>
    public void SetData<T>(T[] data, uint offset = 0)
        where T : unmanaged
    {
        SetData((ReadOnlySpan<T>)data.AsSpan(), offset);
    }

    /// <inheritdoc cref="SetData{T}(T, uint)"/>
    public unsafe void SetData<T>(ReadOnlySpan<T> data, uint offset = 0)
        where T : unmanaged
    {
        fixed (void* handle = data)
            SetData((nint)handle, (uint)(data.Length * Unsafe.SizeOf<T>()), (uint)(offset * Unsafe.SizeOf<T>()));
    }

    /// <summary>
    /// Gets the data from the buffer.
    /// </summary>
    /// <param name="data">The place to store data to.</param>
    /// <param name="size">The size in bytes of the storage.</param>
    /// <param name="offset">The offset where to retrive from the buffer.</param>
    public abstract void GetData(nint data, uint size, uint offset = 0);

    /// <inheritdoc cref="GetData(nint, uint, uint)"/>
    /// <typeparam name="T">The type of data to get the buffer from.</typeparam>
    public void GetData<T>(ref T data, uint offset = 0)
        where T : unmanaged
    {
        GetData(MemoryMarshal.CreateSpan(ref data, 1), offset);
    }

    /// <inheritdoc cref="GetData{T}(ref T, uint)"/>
    public void GetData<T>(T[] data, uint offset = 0)
        where T : unmanaged
    {
        GetData(data.AsSpan(), offset);
    }

    /// <inheritdoc cref="GetData{T}(ref T, uint)"/>
    public unsafe void GetData<T>(Span<T> data, uint offset = 0)
        where T : unmanaged
    {
        fixed (void* handle = data)
            GetData((nint)handle, (uint)(data.Length * Unsafe.SizeOf<T>()), (uint)(offset * Unsafe.SizeOf<T>()));
    }

    public abstract void Dispose();
}
