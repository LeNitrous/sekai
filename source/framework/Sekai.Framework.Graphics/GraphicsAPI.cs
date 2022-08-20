// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// The graphics API to be used.
/// </summary>
public enum GraphicsAPI
{
    /// <summary>
    /// Direct3D 11.
    /// </summary>
    Direct3D11,

    /// <summary>
    /// Vulkan.
    /// </summary>
    Vulkan,

    /// <summary>
    /// OpenGL.
    /// </summary>
    OpenGL,

    /// <summary>
    /// Metal.
    /// </summary>
    Metal,

    /// <summary>
    /// OpenGL ES.
    /// </summary>
    OpenGLES,
}
