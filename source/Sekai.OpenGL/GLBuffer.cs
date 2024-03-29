// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed unsafe class GLBuffer : GraphicsBuffer
{
    public override int Capacity { get; }
    public override BufferType Type { get; }
    public override bool Dynamic { get; }

    private bool isMapped;
    private bool isDisposed;
    private readonly uint handle;
    private readonly BufferTargetARB target;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLBuffer(GL gl, BufferType type, uint size, bool dynamic)
    {
        GL = gl;
        Type = type;
        Dynamic = dynamic;
        Capacity = (int)size;
        target = type.AsTarget();
        handle = GL.GenBuffer();
        GL.BindBuffer(target, handle);
        GL.BufferData(target, size, null, dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw);
    }

    protected override nint Map(MapMode mode)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (isMapped)
        {
            throw new InvalidOperationException("The buffer is already mapped.");
        }

        isMapped = true;

        GL.BindBuffer(target, handle);

        return (nint)GL.MapBuffer(target, mode.AsAccess());
    }

    protected override void Unmap()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (!isMapped)
        {
            return;
        }

        GL.UnmapBuffer(target);

        isMapped = false;
    }

    public override void SetData(nint data, uint size, uint offset)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);
        GL.BindBuffer(target, handle);
        GL.BufferSubData(target, (nint)offset, size, (void*)data);
    }

    public override void GetData(nint data, uint size, uint offset = 0)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);
        GL.BindBuffer(target, handle);
        GL.GetBufferSubData(target, (nint)offset, size, (void*)data);
    }

    public void Bind()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);
        GL.BindBuffer(target, handle);
    }

    public void Bind(uint slot)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLBuffer));
        }

        GL.BindBufferBase(target, slot, handle);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        GL.DeleteBuffer(handle);

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
