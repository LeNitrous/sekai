// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Textures;

namespace Sekai.Graphics.Buffers;

/// <summary>
/// Defines a framebuffer attachment.
/// </summary>
public readonly struct FrameBufferAttachment
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

    public FrameBufferAttachment(Texture target, int layer = 0, int level = 0)
    {
        Layer = layer;
        Level = level;
        Target = target;
    }
}
