// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

[Flags]
public enum BufferUsage : byte
{
    Vertex = 0x1,
    Index = 0x2,
    Uniform = 0x4,
    StructuredReadOnly = 0x8,
    StructuredReadWrite = 0x10,
    Indirect = 0x20,
    Dynamic = 0x40,
    Staging = 0x80,
}

internal static class BufferUsageExtensions
{
    public static Veldrid.BufferUsage ToVeldrid(this BufferUsage usage)
    {
        return (Veldrid.BufferUsage)usage;
    }
}
