// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines the type of buffer.
/// </summary>
public enum BufferType
{
    /// <summary>
    /// This buffer is used as a vertex buffer.
    /// </summary>
    Vertex,

    /// <summary>
    /// This buffer is used as an index buffer.
    /// </summary>
    Index,

    /// <summary>
    /// This buffer is used as a uniform buffer.
    /// </summary>
    Uniform,

    /// <summary>
    /// This buffer is used as a storage buffer.
    /// </summary>
    Storage,

    /// <summary>
    /// This buffer is used as an indirect buffer.
    /// </summary>
    Indirect,
}
