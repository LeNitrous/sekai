// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using BufferDescription = Veldrid.BufferDescription;
using DeviceBuffer = Veldrid.DeviceBuffer;

namespace Sekai.Framework.Graphics.Buffers;

public class Buffer : GraphicsObject<DeviceBuffer>, IBuffer
{
    public int Size { get; }
    public BufferUsage Usage { get; }

    public Buffer(int size, BufferUsage usage)
    {
        Size = size;
        Usage = usage;
    }

    public void SetData(IntPtr data, int offset = 0)
    {
        if (offset > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)offset, data);
    }

    public void SetData(CommandList commands, IntPtr data, int offset = 0)
    {
        if (offset > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, (uint)offset, data);
    }

    protected override DeviceBuffer CreateResource() => Context.Resources.CreateBuffer(new BufferDescription((uint)Size, Usage.ToVeldrid()));
}

public class Buffer<T> : GraphicsObject<DeviceBuffer>, IBuffer
    where T : unmanaged
{
    public int Size => Count * stride;
    public BufferUsage Usage { get; }
    public readonly int Count;

    private static readonly int stride = Marshal.SizeOf<T>();

    public Buffer(int count, BufferUsage usage)
    {
        Count = count;
        Usage = usage;
    }

    public void SetData(T data, int offset = 0)
    {
        SetData(ref data, offset);
    }

    public void SetData(ref T data, int offset = 0)
    {
        if (Size < (offset * Marshal.SizeOf<T>()) || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)(offset * stride), ref data);
    }

    public void SetData(T[] data, int offset = 0)
    {
        if ((offset * stride) + (data.Length * stride) > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)(offset * stride), data);
    }

    public void SetData(CommandList commands, T data, int offset = 0)
    {
        SetData(commands, ref data, offset);
    }

    public void SetData(CommandList commands, ref T data, int offset = 0)
    {
        if (Size < (offset * stride) || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, (uint)(offset * stride), ref data);
    }

    public void SetData(CommandList commands, T[] data, int offset = 0)
    {
        if ((offset * stride) + (data.Length * stride) > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, (uint)(offset * stride), data);
    }

    void IBuffer.SetData(IntPtr data, int offset)
    {
        if (offset > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        Context.Device.UpdateBuffer(Resource, (uint)offset, data);
    }

    void IBuffer.SetData(CommandList commands, IntPtr data, int offset)
    {
        if (offset > Size || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        commands.UpdateBuffer(Resource, (uint)offset, data);
    }

    protected override DeviceBuffer CreateResource() => Context.Resources.CreateBuffer(new BufferDescription((uint)Size, Usage.ToVeldrid()));
}
