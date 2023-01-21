// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Three-dimensional drawables capable of drawing to the <see cref="Scene"/>.
/// </summary>
[Processor<Drawable3DProcessor>]
public abstract class Drawable3D : Drawable, IDrawable
{
    [Bind]
    internal Transform3D Transform { get; set; } = null!;

    Transform IDrawable.Transform => Transform;
}
