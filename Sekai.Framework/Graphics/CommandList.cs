// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Veldrid;

namespace Sekai.Framework.Graphics;

public sealed class CommandList : GraphicsObject<Veldrid.CommandList>
{
    private bool hasStarted;

    internal void Start()
    {
        if (hasStarted)
            throw new InvalidOperationException(@"This command list has already started.");

        hasStarted = true;
        Resource.Begin();
    }

    internal void End()
    {
        if (!hasStarted)
            throw new InvalidOperationException($"{nameof(Start)} must be called before {nameof(End)} can be called.");

        hasStarted = false;
        Resource.End();
        Context.Device.SubmitCommands(Resource);
    }

    internal void EnsureStart()
    {
        if (!hasStarted)
            Start();
    }

    internal void EnsureEnd()
    {
        if (hasStarted)
            End();
    }

    internal void Bind(DeviceBuffer buffer, IndexFormat? format = null)
    {
        if (format.HasValue)
        {
            Resource.SetIndexBuffer(buffer, format.Value);
        }
        else
        {
            Resource.SetVertexBuffer(0, buffer);
        }
    }

    internal void CopyBuffer(DeviceBuffer source, DeviceBuffer destination, int size, int sourceOffset, int destinationOffset)
    {
        if (!hasStarted)
            throw new InvalidOperationException($"{nameof(Start)} must be called before this operation can be started.");

        Resource.CopyBuffer(source, (uint)sourceOffset, destination, (uint)destinationOffset, (uint)size);
    }

    internal void UpdateBuffer(DeviceBuffer buffer, int offset, IntPtr source)
    {
        Resource.UpdateBuffer(buffer, (uint)offset, source);
    }

    internal void UpdateBuffer<T>(DeviceBuffer buffer, int offset, ref T source)
        where T : unmanaged
    {
        Resource.UpdateBuffer(buffer, (uint)offset, source);
    }

    internal void UpdateBuffer<T>(DeviceBuffer buffer, int offset, T[] source)
        where T : unmanaged
    {
        Resource.UpdateBuffer(buffer, (uint)offset, source);
    }

    protected override Veldrid.CommandList CreateResource() => Context.Resources.CreateCommandList();
}
