// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines what is sampled when texture coordinates go outside the boundary.
/// </summary>
public enum TextureAddress
{
    /// <summary>
    /// The texture will repeat.
    /// </summary>
    Repeat,

    /// <summary>
    /// The texture will mirror.
    /// </summary>
    Mirror,

    /// <summary>
    /// The texture will clamp to the edge.
    /// </summary>
    ClampToEdge,

    /// <summary>
    /// The texture will clamp to a predefined border color.
    /// </summary>
    ClampToBorder,
}
