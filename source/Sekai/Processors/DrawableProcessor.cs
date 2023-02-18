// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Rendering;
using Sekai.Scenes;

namespace Sekai.Processors;

internal abstract partial class DrawableProcessor<T> : Processor<T>
    where T : Drawable
{
    protected sealed override void Update(SceneCollection scenes, T drawable) => scenes.Renderer.Collect(drawable);
}

internal sealed class Drawable2DProcessor : DrawableProcessor<Drawable2D>
{
}

internal sealed class Drawable3DProcessor : DrawableProcessor<Drawable3D>
{
}
