// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Buffers;

namespace Sekai.Null.Graphics;

internal class NullBuffer : NativeBuffer
{
    public NullBuffer(int capacity, bool dynamic = false)
        : base(capacity, dynamic)
    {
    }

    public override void GetData(nint dest, int size, int offset = 0)
    {
    }

    public override void SetData(nint data, int size, int offset = 0)
    {
    }
}
