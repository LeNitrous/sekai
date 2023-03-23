// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// Determines how a texture will be used.
/// </summary>
[Flags]
public enum TextureFlag
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The texture is a read-only resource used in an <see cref="Effect"/>.
    /// </summary>
    Sampled = 1 << 0,

    /// <summary>
    /// The texture is a read-write resource used in an <see cref="Effect"/>.
    /// </summary>
    Storage = 1 << 1,

    /// <summary>
    /// The texture is ued as a cubemap.
    /// </summary>
    Cubemap = 1 << 2,
}
