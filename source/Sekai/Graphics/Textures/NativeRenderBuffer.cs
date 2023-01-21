// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

public readonly struct NativeRenderBuffer
{
    public readonly int Layer;
    public readonly int Level;
    public readonly NativeTexture Texture;

    public NativeRenderBuffer(int layer, int level, NativeTexture texture)
    {
        Layer = layer;
        Level = level;
        Texture = texture;
    }

    public static implicit operator NativeRenderBuffer(RenderBuffer buffer) => new(buffer.Layer, buffer.Layer, buffer.Target.Native);
}
