// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridBuffer : VeldridBindableResource<Vd.DeviceBuffer>, IBuffer
{
    public uint Size { get; }
    public BufferUsage Usage { get; }

    public VeldridBuffer(BufferDescription desc, Vd.DeviceBuffer buffer)
        : base(buffer)
    {
        Size = desc.Size;
        Usage = desc.Usage;
    }
}
