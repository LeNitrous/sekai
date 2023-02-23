// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;
using Sekai.Rendering;

namespace Sekai.Processors;

internal abstract partial class DrawableProcessor<T> : Processor<T>
    where T : Drawable
{
    [Resolved]
    private Renderer renderer { get; set; } = null!;

    protected sealed override void Update(T drawable) => renderer.Collect((IRenderObject)drawable);
}

internal sealed class Drawable2DProcessor : DrawableProcessor<Drawable2D>
{
}

internal sealed class Drawable3DProcessor : DrawableProcessor<Drawable3D>
{
}
