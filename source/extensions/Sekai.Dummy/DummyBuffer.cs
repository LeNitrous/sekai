// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Dummy;

internal class DummyBuffer : FrameworkObject, IBuffer
{
    public uint Size { get; }
    public BufferUsage Usage { get; }

    public DummyBuffer(uint size, BufferUsage usage)
    {
        Size = size;
        Usage = usage;
    }
}
