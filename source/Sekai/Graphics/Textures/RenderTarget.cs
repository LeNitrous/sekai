// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Allocation;

namespace Sekai.Graphics.Textures;

/// <summary>
/// Contains data necessary for blitting an image onto the screen.
/// </summary>
public sealed class RenderTarget : GraphicsObject, IRenderTarget
{
    public int Width { get; }

    public int Height { get; }

    /// <summary>
    /// The depth attachment of this framebuffer.
    /// </summary>
    public readonly RenderBuffer? Depth;

    /// <summary>
    /// THe color attachments of this framebuffer.
    /// </summary>
    public readonly IReadOnlyList<RenderBuffer> Color;

    private readonly NativeRenderTarget native;

    public RenderTarget(RenderBuffer color, RenderBuffer? depth = null)
        : this(new[] { color }, depth)
    {
    }

    public RenderTarget(IReadOnlyList<RenderBuffer> color, RenderBuffer? depth = null)
    {
        if (color.Count > 8)
            throw new ArgumentException(@"There cannot be more than 8 color attachments in a framebuffer.", nameof(color));

        native = Graphics.CreateRenderTarget(color.Cast<NativeRenderBuffer>().ToArray(), depth);

        Color = color;
        Depth = depth;

        Width = Math.Max(Depth?.Target.Width ?? 0, Color.Max(b => b.Target.Width));
        Height = Math.Max(Depth?.Target.Height ?? 0, Color.Max(b => b.Target.Height));
    }

    protected override void DestroyGraphics() => native.Dispose();

    NativeRenderTarget IRenderTarget.Native => native;
}
