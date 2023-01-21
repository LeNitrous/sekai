// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Rendering;

namespace Sekai.Processors;

internal sealed class Drawable2DProcessor : DrawableProcessor<Drawable2D>
{
    protected override void Update(Drawable2D drawable) => Renderer.Collect(drawable);
}
