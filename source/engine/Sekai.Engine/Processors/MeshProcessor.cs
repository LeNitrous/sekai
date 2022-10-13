// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Processors;

public sealed class MeshProcessor : Processor<MeshComponent, Transform>
{
    public MeshProcessor()
    {
        OnEntityAdded += handleEntityAdded;
        OnEntityRemoved += handleEntityRemoved;
    }

    protected override void Update(double delta, Entity entity, MeshComponent componentA, Transform componentB)
    {
    }

    private void handleEntityAdded(Processor processor, Entity entity)
    {
        Scene.RenderContext.Add(entity.GetCommponent<MeshComponent>()!);
    }

    private void handleEntityRemoved(Processor processor, Entity entity)
    {
        Scene.RenderContext.Remove(entity.GetCommponent<MeshComponent>()!);
    }

    protected override void Destroy()
    {
        OnEntityAdded -= handleEntityAdded;
        OnEntityRemoved -= handleEntityRemoved;
        base.Destroy();
    }
}
