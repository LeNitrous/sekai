// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.


using Sekai.Engine.Rendering;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

public abstract class Renderable : Component, IRenderable
{
    [Resolved]
    private SystemCollection<SceneSystem> systems { get; set; } = null!;

    protected RenderContext RenderContext { get; private set; } = null!;

    public abstract void Render();

    private protected override void OnComponentLoad()
    {
        RenderContext = systems.Get<RenderContext>();
    }

    private protected override void OnComponentActivate()
    {
        // RenderContext.Add(this);
    }

    private protected override void OnComponentDeactivate()
    {
        // RenderContext.Remove(this);
    }
}
