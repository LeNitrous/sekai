// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Rendering;

namespace Sekai.Engine.Processors;

public class MeshProcessor : Processor<MeshComponent, Transform>
{
    private RenderContext renderContext = null!;

    protected override void Load()
    {
        renderContext = Systems.Get<RenderContext>();
    }

    protected override void OnEntityAdded(Entity entity, MeshComponent componentA, Transform componentB)
    {
        renderContext.Add(componentA);
    }

    protected override void OnEntityRemoved(Entity entity, MeshComponent componentA, Transform componentB)
    {
        renderContext.Remove(componentA);
    }

    protected override void Update(double elapsed, Entity entity, MeshComponent componentA, Transform componentB)
    {
    }
}
