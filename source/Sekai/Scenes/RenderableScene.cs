// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;
using Sekai.Rendering;

namespace Sekai.Scenes;

public abstract class RenderableScene<T> : Scene, IRenderableScene<T>
    where T : Node, IRenderableNode
{
    public new T Root => (T)base.Root;

    internal Renderer Renderer { get; }

    public RenderableScene()
    {
        Renderer = CreateRenderer();
    }

    public void Render(GraphicsContext graphics) => Renderer.Render(graphics);

    protected abstract Renderer CreateRenderer();
}
