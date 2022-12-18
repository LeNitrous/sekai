// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Buffers;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal unsafe class GLBuffer : GLResource, INativeBuffer
{
    public int Capacity { get; }
    private readonly uint bufferId;

    public GLBuffer(GLGraphicsSystem context, int capacity, bool dynamic)
        : base(context)
    {
        Capacity = capacity;
        bufferId = GL.GenBuffer();

        GL.BindBuffer(BufferTargetARB.CopyWriteBuffer, bufferId);
        GL.BufferData(BufferTargetARB.CopyWriteBuffer, (nuint)capacity, null, dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw);
    }

    public void GetData(nint dest, int size, int offset = 0)
    {
        GL.BindBuffer(BufferTargetARB.CopyWriteBuffer, bufferId);
        GL.GetBufferSubData(BufferTargetARB.CopyWriteBuffer, offset, (nuint)size, (void*)dest);
    }

    public void SetData(nint data, int size, int offset = 0)
    {
        GL.BindBuffer(BufferTargetARB.CopyWriteBuffer, bufferId);
        GL.BufferSubData(BufferTargetARB.CopyWriteBuffer, offset, (nuint)size, (void*)data);
    }

    protected override void Destroy()
    {
        GL.DeleteBuffer(bufferId);
    }

    public static implicit operator uint(GLBuffer buffer) => buffer.bufferId;
}
