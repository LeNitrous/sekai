// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Sekai.Graphics;

/// <summary>
/// Arguments used for indirect rendering.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct IndirectDrawArguments
{
    /// <summary>
    /// The number of vertices.
    /// </summary>
    public uint VertexCount;

    /// <summary>
    /// The number of instances to be drawn.
    /// </summary>
    public uint InstanceCount;

    /// <summary>
    /// An offset in the vertex buffer.
    /// </summary>
    public uint VertexOffset;

    /// <summary>
    /// The starting instance number.
    /// </summary>
    public uint InstanceOffset;
}
