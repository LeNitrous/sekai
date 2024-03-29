// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of graphics APIs.
/// </summary>
public enum GraphicsAPI
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// OpenGL.
    /// </summary>
    /// <remarks>
    /// OpenGL-ES is used on mobile platforms.
    /// </remarks>
    OpenGL,

    /// <summary>
    /// Direct 3D 11.
    /// </summary>
    /// <remarks>
    /// Available on Windows only.
    /// </remarks>
    D3D11,

    /// <summary>
    /// Vulkan.
    /// </summary>
    Vulkan,

    /// <summary>
    /// Metal.
    /// </summary>
    /// <remarks>
    /// Available on Apple platforms only.
    /// </remarks>
    Metal,

    /// <summary>
    /// WebGPU.
    /// </summary>
    WebGPU,

    /// <summary>
    /// Dummy.
    /// </summary>
    Dummy = int.MaxValue,
}
