// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Services;

public sealed class ComponentService : FrameworkObject, IGameService
{
    private readonly Queue<Component> components = new();

    public void Update(double elapsed)
    {
        if (!components.TryDequeue(out var component))
            return;

        component.Initialize();
    }

    public void FixedUpdate()
    {
    }

    public void Render()
    {
    }

    internal void Add(Component component)
    {
        components.Enqueue(component);
    }
}
