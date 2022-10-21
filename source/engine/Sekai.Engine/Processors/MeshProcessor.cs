// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Engine.Rendering;

namespace Sekai.Engine.Processors;

public sealed class MeshProcessor : Processor<MeshComponent, Transform>
{
    private readonly Dictionary<MeshComponent, MeshRenderObject> renderables = new();

    protected override void OnEntityAdded(Entity entity, MeshComponent mesh, Transform transform)
    {
        if (renderables.ContainsKey(mesh))
            return;

        var renderable = new MeshRenderObject { Mesh = mesh.Mesh };
        renderables.Add(mesh, renderable);
        Scene.RenderContext.Add(renderable);
    }

    protected override void OnEntityRemoved(Entity entity, MeshComponent mesh, Transform transform)
    {
        if (renderables.Remove(mesh, out var renderable))
            Scene.RenderContext.Remove(renderable);
    }

    protected override void Update(Entity entity, MeshComponent mesh, Transform transform)
    {
        renderables[mesh].WorldMatrix = transform.WorldMatrix;
    }
}
