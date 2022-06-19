// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sekai.Framework.Extensions;
using Veldrid;
using BufferDescription = Veldrid.BufferDescription;
using DeviceBuffer = Veldrid.DeviceBuffer;

namespace Sekai.Framework.Graphics.Buffers;

/// <summary>
/// Represents a buffer of memory in the GPU.
/// </summary>
public class Buffer : MappableObject<DeviceBuffer>
{
    /// <summary>
    /// The size of this buffer in bytes.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// The flags that indicate how this buffer is used.
    /// </summary>
    public BufferUsage Usage { get; }

    public Buffer(int size, BufferUsage usage)
    {
        Size = size;
        Usage = usage;
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
    /// Sets the data at a given offset with a provided command list.
    /// </summary>
    public void SetData(CommandList commands, nint data, int offset = 0)
    {
        if (offset > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, offset, data);
    }

    /// <summary>
    /// Gets the data and stores it to the provided pointer.
    /// </summary>
    public unsafe void GetData(CommandList commands, nint data)
    {
        commands.EnsureStart();

        using var staging = new Buffer(Size, BufferUsage.Staging);
        commands.CopyBuffer(Resource, staging.Resource, Size, 0, 0);
        commands.End();

        using var mapped = staging.Map(MapMode.Read);
        System.Buffer.MemoryCopy((void*)mapped.Data, (void*)data, Size, Size);
        commands.Start();
    }

    internal override MappedResource Map(MapMode mode) => new(this, mode);
    protected override DeviceBuffer CreateResource() => Context.Resources.CreateBuffer(new BufferDescription((uint)Size, Usage.ToVeldrid()));
}

/// <inheritdoc cref="Buffer"/>
public class Buffer<T> : Buffer, IBindable
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

    public void Bind(CommandList commands)
    {
        if (Usage.HasFlag(BufferUsage.Index))
        {
            if (index_format_map.TryGetValue(typeof(T), out var format))
            {
                commands.Bind(Resource, format);
                return;
            }

            throw new InvalidOperationException(@"Index buffer has invalid type.");
        }

        if (Usage.HasFlag(BufferUsage.Vertex))
        {
            commands.Bind(Resource);
            return;
        }

        throw new InvalidOperationException(@"Buffer must be an index or vertex buffer to be bound.");
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

    /// <inheritdoc cref="Buffer.SetData(nint, int)"/>
    public void SetData(CommandList commands, T data, int offset = 0)
    {
        SetData(commands, ref data, offset);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int)"/>
    public void SetData(CommandList commands, ref T data, int offset = 0)
    {
        if (Size < (offset * stride) || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, offset * stride, ref data);
    }

    /// <inheritdoc cref="Buffer.SetData(nint, int)"/>
    public void SetData(CommandList commands, T[] data, int offset = 0)
    {
        if ((offset * stride) + (data.Length * stride) > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, offset * stride, data);
    }

    /// <summary>
    /// Gets the data and stores it to the reference.
    /// </summary>
    public unsafe void GetData(CommandList commands, ref T data)
    {
        commands.EnsureStart();

        using var staging = new Buffer(Size, BufferUsage.Staging);
        commands.CopyBuffer(Resource, staging.Resource, Size, 0, 0);
        commands.End();

        using var mapped = staging.Map(MapMode.Read);
        fixed (byte* ptr = &Unsafe.AsRef<byte>(Unsafe.AsPointer(ref data)))
        {
            System.Buffer.MemoryCopy((void*)mapped.Data, ptr, Size, Size);
        }

        commands.Start();
    }

    /// <summary>
    /// Gets the data and stores it to the array.
    /// </summary>
    public unsafe void GetData(CommandList commands, T[] data)
    {
        commands.EnsureStart();

        using var staging = new Buffer(Size, BufferUsage.Staging);
        commands.CopyBuffer(Resource, staging.Resource, data.Length * stride, 0, 0);
        commands.End();

        using var mapped = staging.Map(MapMode.Read);
        fixed (byte* ptr = &Unsafe.AsRef<byte>(Unsafe.AsPointer(ref data[0])))
        {
            System.Buffer.MemoryCopy((void*)mapped.Data, ptr, data.Length * stride, data.Length * stride);
        }

        commands.Start();
    }

    private static readonly Dictionary<Type, IndexFormat> index_format_map = new()
    {
        { typeof(int), IndexFormat.UInt32 },
        { typeof(uint), IndexFormat.UInt32 },
        { typeof(short), IndexFormat.UInt16 },
        { typeof(ushort), IndexFormat.UInt16 },
    };
}
