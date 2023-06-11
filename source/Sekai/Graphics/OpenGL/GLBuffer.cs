// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

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

    public override nint Map(MapMode mode)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLBuffer));
        }

        if (isMapped)
        {
            throw new InvalidOperationException("The buffer is already mapped.");
        }

        isMapped = true;

        GL.BindBuffer(target, handle);

        return (nint)GL.MapBuffer(target, mode.AsAccess());
    }

    public override void Unmap()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLBuffer));
        }

        if (!isMapped)
        {
            return;
        }

        GL.UnmapBuffer(target);

        isMapped = false;
    }

    public override void SetData(nint data, uint size, uint offset)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLBuffer));
        }

        GL.BindBuffer(target, handle);
        GL.BufferSubData(target, (nint)offset, size, (void*)data);
    }

    public override void GetData(nint data, uint size, uint offset = 0)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLBuffer));
        }

        GL.BindBuffer(target, handle);
        GL.GetBufferSubData(target, (nint)offset, size, (void*)data);
    }

    public void Bind()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLBuffer));
        }

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

    ~GLBuffer()
    {
        Dispose();
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
