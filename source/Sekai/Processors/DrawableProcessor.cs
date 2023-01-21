// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;
using Sekai.Rendering;

namespace Sekai.Processors;

internal abstract class DrawableProcessor<T> : Processor<T>
    where T : Drawable
{
    [Resolved]
    protected Renderer Renderer { get; private set; } = null!;
}
