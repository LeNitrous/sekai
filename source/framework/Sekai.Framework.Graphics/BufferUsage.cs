// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// A bitmask describing the permitted uses of a <see cref="IBuffer"/> object.
/// </summary>
[Flags]
public enum BufferUsage : byte
{
    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> can be used as the source of vertex data for drawing commands.
    /// This flag enables the use of a Buffer in the <see cref="CommandList.SetVertexBuffer(uint, IBuffer)"/> method.
    /// </summary>
    VertexBuffer = 1 << 0,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> can be used as the source of index data for drawing commands.
    /// This flag enables the use of a Buffer in the <see cref="CommandList.SetIndexBuffer(IBuffer, IndexFormat)" /> method.
    /// </summary>
    IndexBuffer = 1 << 1,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> can be used as a uniform Buffer.
    /// This flag enables the use of a Buffer in a <see cref="IResourceSet"/> as a uniform Buffer.
    /// </summary>
    UniformBuffer = 1 << 2,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> can be used as a read-only structured Buffer.
    /// This flag enables the use of a Buffer in a <see cref="IResourceSet"/> as a read-only structured Buffer.
    /// This flag can only be combined with <see cref="Dynamic"/>.
    /// </summary>
    StructuredBufferReadOnly = 1 << 3,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> can be used as a read-write structured Buffer.
    /// This flag enables the use of a Buffer in a <see cref="IResourceSet"/> as a read-write structured Buffer.
    /// This flag cannot be combined with any other flag.
    /// </summary>
    StructuredBufferReadWrite = 1 << 4,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> can be used as the source of indirect drawing information.
    /// This flag enables the use of a Buffer in the *Indirect methods of <see cref="CommandList"/>.
    /// This flag cannot be combined with <see cref="Dynamic"/>.
    /// </summary>
    IndirectBuffer = 1 << 5,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> will be updated with new data very frequently. Dynamic Buffers can be
    /// mapped with <see cref="MapMode.Write"/>. This flag cannot be combined with <see cref="StructuredBufferReadWrite"/>
    /// or <see cref="IndirectBuffer"/>.
    /// </summary>
    Dynamic = 1 << 6,

    /// <summary>
    /// Indicates that a <see cref="IBuffer"/> will be used as a staging Buffer. Staging Buffers can be used to transfer data
    /// to-and-from the CPU using <see cref="IGraphicsContext.Map(IBuffer, MapMode)"/>. Staging Buffers can use all
    /// <see cref="MapMode"/> values.
    /// This flag cannot be combined with any other flag.
    /// </summary>
    Staging = 1 << 7,
}
