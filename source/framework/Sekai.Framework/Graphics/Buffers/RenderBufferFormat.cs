// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics.Buffers;

/// <summary>
/// Determines a render buffer's format.
/// </summary>
public enum RenderBufferFormat
{
    /// <summary>
    /// 16-bit depth format.
    /// </summary>
    D16,

    /// <summary>
    /// 32-bit depth format.
    /// </summary>
    D32,

    /// <summary>
    /// 24-bit depth + 8-bit stencil format.
    /// </summary>
    D24S8,

    /// <summary>
    /// 32-bit depth + 8-bit stencil format.
    /// </summary>
    D32S8,
}
