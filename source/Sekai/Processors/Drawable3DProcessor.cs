// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Rendering;

namespace Sekai.Processors;

internal sealed class Drawable3DProcessor : DrawableProcessor<Drawable3D>
{
    protected override void Update(Drawable3D drawable) => Renderer.Collect(drawable);
}
