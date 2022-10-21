// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public abstract class BindableBuffer : FrameworkObject
{
    public int Size { get; }

    protected readonly IBuffer Buffer;
    protected readonly IGraphicsDevice Device;

    protected BindableBuffer(IGraphicsDevice device, int size, BufferUsage usage)
    {
        if (size <= 0)
            throw new ArgumentException($"Size must be greater than zero.");

        Size = size;
        Device = device;

        var description = new BufferDescription((uint)size, usage);
        Buffer = device.Factory.CreateBuffer(ref description);
    }

    public abstract void Bind(ICommandQueue queue);

    /// <summary>
    /// Sets the data for this graphics buffer.
    /// </summary>
    public void SetData(nint data, int size, int offset = 0)
    {
        Device.UpdateBufferData(Buffer, data, (uint)size, (uint)offset);
    }

    protected override void Destroy()
    {
        Buffer.Dispose();
    }
}
