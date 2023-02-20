// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Graphics.Textures;

/// <summary>
/// Contains data necessary for blitting an image onto the screen.
/// </summary>
public sealed class RenderTarget : ServiceableGraphicsObject<NativeRenderTarget>, IRenderTarget
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

    public RenderTarget(RenderBuffer color, RenderBuffer? depth = null)
        : this(new[] { color }, depth)
    {
    }

    public RenderTarget(IReadOnlyList<RenderBuffer> color, RenderBuffer? depth = null)
        : base(context => context.CreateRenderTarget(color, depth))
    {
        Color = color;
        Depth = depth;

        Width = Math.Max(Depth?.Target.Width ?? 0, Color.Max(b => b.Target.Width));
        Height = Math.Max(Depth?.Target.Height ?? 0, Color.Max(b => b.Target.Height));
    }

    NativeRenderTarget? IRenderTarget.Native => Native;
}
