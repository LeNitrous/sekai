// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Indicates how a <see cref="MappedResource"/> will be accessible to the CPU.
/// </summary>
[Flags]
public enum MapMode : byte
{
    /// <summary>
    /// Indicates that the resource will be read-only. This mode can only be used on resources with the
    /// <see cref="Buffers.BufferUsage.Staging"/> flag. Can be combined with <see cref="Write"/> for read-write access.
    /// </summary>
    Read,

    /// <summary>
    /// Indicates tha the resource will be write-only. Can be combined with <see cref="Read"/>
    /// for read-write access.
    /// </summary>
    Write,
}
