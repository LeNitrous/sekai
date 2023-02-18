// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Two-dimensional drawables capable of drawing to the <see cref="Scene"/>.
/// </summary>
[Processor<Drawable2DProcessor>]
public abstract partial class Drawable2D : Drawable
{
    [Bind]
    internal Transform2D Transform { get; private set; } = null!;

    internal sealed override SceneKind Kind => SceneKind.Scene2D;

    internal sealed override Transform GetTransform() => Transform;
}
