// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Sekai.Framework.Extensions;
using Veldrid;
using BufferDescription = Veldrid.BufferDescription;
using DeviceBuffer = Veldrid.DeviceBuffer;

namespace Sekai.Framework.Graphics.Buffers;

/// <summary>
/// Represents a buffer of memory in the GPU.
/// </summary>
public class Buffer : GraphicsObject<DeviceBuffer>, IBindableResource
{
    /// <summary>
    /// The size of this buffer in bytes.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// The flags that indicate how this buffer is used.
    /// </summary>
    public BufferUsage Usage { get; }

    internal override DeviceBuffer Resource { get; }

    public Buffer(int size, BufferUsage usage)
    {
        Size = size;
        Usage = usage;
        Resource = Context.Resources.CreateBuffer(new BufferDescription((uint)Size, Usage.ToVeldrid()));
    }

    /// <summary>
    /// Sets the data at a given offset.
    /// </summary>
    public void SetData(nint data, int offset = 0)
    {
        if (offset > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)offset, data);
    }

    /// <summary>
    /// Gets the data and stores it to the provided pointer.
    /// </summary>
    public unsafe void GetData(nint data)
    {
        using var commands = new CommandList();
        commands.Start();

        using var staging = new Buffer(Size, BufferUsage.Staging);
        commands.CopyBuffer(Resource, staging.Resource, Size, 0, 0);
        commands.End();

        using var mapped = new BufferMappedResource(this, MapMode.Read);
        System.Buffer.MemoryCopy((void*)mapped.Data, (void*)data, Size, Size);
    }

    BindableResource IBindableResource.Resource => Resource;

    internal class BufferMappedResource : MappedResource
    {
        internal BufferMappedResource(Buffer buffer, MapMode mode)
            : base(buffer.Resource, mode)
        {
        }
    }
}

/// <inheritdoc cref="Buffer"/>
public class Buffer<T> : Buffer
    where T : unmanaged
{
    /// <summary>
    /// The number of elements this buffer contains.
    /// </summary>
    public readonly int Count;

    private static readonly int stride = Unsafe.SizeOf<T>();

    public Buffer(int count, BufferUsage usage)
        : base(count * stride, usage)
    {
        Count = count;
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int)"/>
    public void SetData(T data, int offset = 0)
    {
        SetData(ref data, offset);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int)"/>
    public void SetData(ref T data, int offset = 0)
    {
        if (Size < (offset * stride) || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)(offset * stride), ref data);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int)"/>
    public void SetData(T[] data, int offset = 0)
    {
        if ((offset * stride) + (data.Length * stride) > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)(offset * stride), data);
    }

    /// <summary>
    /// Gets the data and stores it to the reference.
    /// </summary>
    public unsafe void GetData(ref T data)
    {
        using var commands = new CommandList();
        commands.Start();

        using var staging = new Buffer(Size, BufferUsage.Staging);
        commands.CopyBuffer(Resource, staging.Resource, Size, 0, 0);
        commands.End();

        using var mapped = new BufferMappedResource(this, MapMode.Read);
        fixed (byte* ptr = &Unsafe.As<T, byte>(ref data))
        {
            System.Buffer.MemoryCopy((void*)mapped.Data, ptr, Size, Size);
        }
    }

    /// <summary>
    /// Gets the data and stores it to the array.
    /// </summary>
    public unsafe void GetData(T[] data)
    {
        using var commands = new CommandList();
        commands.Start();

        using var staging = new Buffer(Size, BufferUsage.Staging);
        commands.CopyBuffer(Resource, staging.Resource, data.Length * stride, 0, 0);
        commands.End();

        using var mapped = new BufferMappedResource(this, MapMode.Read);
        fixed (byte* ptr = &Unsafe.As<T, byte>(ref data[0]))
        {
            System.Buffer.MemoryCopy((void*)mapped.Data, ptr, data.Length * stride, data.Length * stride);
        }
    }
}
