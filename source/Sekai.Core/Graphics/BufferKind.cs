// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of buffer types according to its use.
/// </summary>
public enum BufferKind
{
    /// <summary>
    /// Index buffers used to store index data.
    /// </summary>
    Index,

    /// <summary>
    /// Vertex buffers used to store vertex data.
    /// </summary>
    Vertex,

    /// <summary>
    /// Structured read-only buffers used to store structured data.
    /// </summary>
    StructuredReadOnly,

    /// <summary>
    /// Structured buffers used to used to store structured data.
    /// </summary>
    StructuredReadWrite,
}
