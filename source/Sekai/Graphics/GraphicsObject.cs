// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;

namespace Sekai.Graphics;

public abstract class GraphicsObject : DependencyObject
{
    [Resolved]
    protected GraphicsSystem Graphics { get; set; } = null!;

    [Resolved]
    private GraphicsContext context { get; set; } = null!;

    protected sealed override void Destroy() => context.Schedule(DestroyGraphics);

    protected virtual void DestroyGraphics()
    {
    }
}
