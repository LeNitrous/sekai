// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Three-dimensional drawables capable of drawing to the <see cref="Scene"/>.
/// </summary>
[Processor<Drawable3DProcessor>]
public abstract partial class Drawable3D : Drawable
{
    [Bind]
    internal Transform3D Transform { get; private set; } = null!;

    internal sealed override SceneKind Kind => SceneKind.Scene3D;

    internal sealed override Transform GetTransform() => Transform;
}
