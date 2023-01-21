// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Two-dimensional drawables capable of drawing to the <see cref="Scene"/>.
/// </summary>
[Processor<Drawable2DProcessor>]
public abstract class Drawable2D : Drawable, IDrawable
{
    [Bind]
    internal Transform2D Transform { get; set; } = null!;

    Transform IDrawable.Transform => Transform;
}
