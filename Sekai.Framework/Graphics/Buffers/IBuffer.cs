// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Buffers;

public interface IBuffer
{
    int Size { get; }
    BufferUsage Usage { get; }
    void SetData(IntPtr data, int offset = 0);
    void SetData(CommandList commands, IntPtr data, int offset = 0);
}
