// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Buffers;

/// <summary>
/// Flags denoting the permitted uses of a <see cref="Buffer"/>.
/// </summary>
[Flags]
public enum BufferUsage : byte
{
    /// <summary>
    /// The buffer is bindable as a vertex buffer.
    /// </summary>
    Vertex = 0x1,

    /// <summary>
    /// The buffer is bindable as an index buffer.
    /// </summary>
    Index = 0x2,

    /// <summary>
    /// The buffer is usable as a shader resource.
    /// </summary>
    Uniform = 0x4,

    /// <summary>
    /// The buffer is a read-only structured buffer. This flag can only be combined with <see cref="Dynamic"/>.
    /// </summary>
    StructuredReadOnly = 0x8,

    /// <summary>
    /// The buffer is a read-write structured buffer. This flag cannot be combined with any other flag.
    /// </summary>
    StructuredReadWrite = 0x10,

    /// <summary>
    /// The buffer can be used as a source for indirect drawing information.
    /// </summary>
    Indirect = 0x20,

    /// <summary>
    /// The buffer is indicated that it will be updated with new data frequently. Buffers with this flag can be
    /// mapped with <see cref="MapMode.Write"/>. This flag cannot be combined with <see cref="StructuredReadWrite"/> or <see cref="Indirect"/>.
    /// </summary>
    Dynamic = 0x40,

    /// <summary>
    /// The buffer is indicated that it will be used for staging. Staging buffers can be mapped and can use any <see cref="MapMode"/> values.
    /// This flag cannot be combined with any other flag.
    /// </summary>
    Staging = 0x80,
}
