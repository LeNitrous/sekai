// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

/// <summary>
/// Defines a render target's buffer.
/// </summary>
public readonly struct RenderBuffer
{
    /// <summary>
    /// The target texture layer to draw to.
    /// </summary>
    public readonly int Layer;

    /// <summary>
    /// The target texture mip level to draw to.
    /// </summary>
    public readonly int Level;

    /// <summary>
    /// The target texture to draw to.
    /// </summary>
    public readonly Texture Target;

    public RenderBuffer(Texture target, int layer = 0, int level = 0)
    {
        Layer = layer;
        Level = level;
        Target = target;
    }
}
