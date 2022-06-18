// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Veldrid;

namespace Sekai.Framework.Graphics;

public class CommandList : GraphicsObject<Veldrid.CommandList>
{
    internal void Begin()
    {
        Resource.Begin();
    }

    internal void End()
    {
        Resource.End();
        Context.Device.SubmitCommands(Resource);
    }

    internal void UpdateBuffer(DeviceBuffer buffer, uint offset, IntPtr source)
    {
        Resource.UpdateBuffer(buffer, offset, source);
    }

    internal void UpdateBuffer<T>(DeviceBuffer buffer, uint offset, ref T source)
        where T : unmanaged
    {
        Resource.UpdateBuffer(buffer, offset, source);
    }

    internal void UpdateBuffer<T>(DeviceBuffer buffer, uint offset, T[] source)
        where T : unmanaged
    {
        Resource.UpdateBuffer(buffer, offset, source);
    }

    protected sealed override Veldrid.CommandList CreateResource() => Context.Resources.CreateCommandList();
}
