// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Rendering;
using Sekai.Scenes;

namespace Sekai.Processors;

public abstract class TransformProcessor<T> : Processor, IUpdateable
    where T : Node, IRenderableNode
{
    private readonly List<T> nodes = new();

    public void Update(double delta)
    {
        foreach (var node in nodes)
            Update(node);
    }

    internal abstract void Update(T node);

    internal void Add(T node)
    {
        if (!nodes.Contains(node))
            nodes.Add(node);
    }

    internal void Remove(T node)
    {
        nodes.Remove(node);
    }
}
