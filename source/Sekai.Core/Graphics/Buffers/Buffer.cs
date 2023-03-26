// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sekai.Graphics.Buffers;

/// <summary>
/// A resource used to store arbitrary data that can be sent to the graphics device.
/// </summary>
public partial class Buffer : IDisposable
{
    /// <summary>
    /// The buffer's capacity in bytes.
    /// </summary>
    public int Capacity => (int)buffer.SizeInBytes;

    /// <summary>
    /// The buffer's usage kind.
    /// </summary>
    public BufferKind Kind { get; }

    /// <summary>
    /// Whether this buffer is optimized for constant data updates.
    /// </summary>
    public bool Dynamic { get; }

    private bool isDisposed;
    private readonly Veldrid.DeviceBuffer buffer;

    internal Buffer(Veldrid.DeviceBuffer buffer, BufferKind kind, bool dynamic)
    {
        Kind = kind;
        Dynamic = dynamic;
        this.buffer = buffer;
    }

    /// <summary>
    /// Sets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The source data pointer.</param>
    /// <param name="size">The size of the source data in bytes.</param>
    /// <param name="offset">The offset where to start writing in bytes.</param>
    /// <exception cref="ArgumentException"></exception>
    public void SetData(GraphicsDevice device, nint data, int size, int offset = 0)
    {
        if (offset < 0 || size + offset > Capacity)
        {
            throw new ArgumentException("Attempted to write data out of bounds.");
        }

        device.Veldrid.UpdateBuffer(buffer, (uint)offset, data, (uint)size);
    }

    /// <summary>
    /// Gets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The destination data pointer.</param>
    /// <param name="size">The size of the destination data pointer in bytes.</param>
    public unsafe void GetData(GraphicsDevice device, nint data, int size)
    {
        using var staging = device.Factory.CreateBuffer(new((uint)Capacity, Veldrid.BufferUsage.Staging));
        using var command = device.Factory.CreateCommandList();
        command.Begin();
        command.CopyBuffer(buffer, 0, staging, 0, (uint)Capacity);
        command.End();

        device.Veldrid.SubmitCommands(command);
        device.Veldrid.WaitForIdle();

        var mapped = device.Veldrid.Map(staging, Veldrid.MapMode.Read);
        Unsafe.CopyBlock(data.ToPointer(), mapped.Data.ToPointer(), Math.Min((uint)size, mapped.SizeInBytes));
        device.Veldrid.Unmap(staging);
    }

    ~Buffer()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// A strongly-typed <see cref="Buffer"/> where arbitrary data can be stored that can be sent to a graphics device.
/// </summary>
/// <typeparam name="T">The blittable type.</typeparam>
public class Buffer<T> : Buffer
    where T : unmanaged
{
    /// <summary>
    /// The buffer's capacity in elements.
    /// </summary>
    public int Length => Capacity / Unsafe.SizeOf<T>();

    internal Buffer(Veldrid.DeviceBuffer buffer, BufferKind kind, bool dynamic)
        : base(buffer, kind, dynamic)
    {
    }

    /// <summary>
    /// Sets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="offset">The offset where to start writing in bytes.</param>
    public void SetData(GraphicsDevice device, T data, int offset = 0)
    {
        SetData(device, ref data, 1, offset);
    }

    /// <summary>
    /// Sets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The data to set.</param>
    /// <param name="length">The number of elements to copy from source.</param>
    /// <param name="offset">The offset where to start writing in bytes.</param>
    public void SetData(GraphicsDevice device, ref T data, int length = 1, int offset = 0)
    {
        SetData(device, MemoryMarshal.CreateReadOnlySpan(ref data, length), offset);
    }

    /// <summary>
    /// Sets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The array of data to set.</param>
    /// <param name="offset">The offset where to start writing in bytes.</param>
    public void SetData(GraphicsDevice device, T[] data, int offset = 0)
    {
        SetData(device, (ReadOnlySpan<T>)data.AsSpan(), offset);
    }

    /// <summary>
    /// Sets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The span of data to set.</param>
    /// <param name="offset">The offset where to start writing in bytes.</param>
    public unsafe void SetData(GraphicsDevice device, ReadOnlySpan<T> data, int offset = 0)
    {
        fixed (T* ptr = data)
        {
            SetData(device, (nint)ptr, data.Length * Unsafe.SizeOf<T>(), offset);
        }
    }

    /// <summary>
    /// Gets the data of this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <returns>The stored data.</returns>
    public T GetData(GraphicsDevice device)
    {
        T data = default;
        GetData(device, ref data);
        return data;
    }

    /// <summary>
    /// Gets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The stored data.</param>
    /// <param name="length">The number of elements to retrieve.</param>
    public void GetData(GraphicsDevice device, ref T data, int length = 1)
    {
        GetData(device, MemoryMarshal.CreateSpan(ref data, length));
    }

    /// <summary>
    /// Gets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The destination array where to store the retrieved data.</param>
    public void GetData(GraphicsDevice device, T[] data)
    {
        GetData(device, data.AsSpan());
    }

    /// <summary>
    /// Gets the data for this buffer.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="data">The destination span where to store the retrieved data.</param>
    public unsafe void GetData(GraphicsDevice device, Span<T> data)
    {
        fixed (T* ptr = data)
        {
            GetData(device, (nint)ptr, data.Length * Unsafe.SizeOf<T>());
        }
    }
}
