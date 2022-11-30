// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Processors;

public sealed class ScriptableProcessor : Processor, IUpdateable
{
    private readonly Queue<Scriptable> scripts = new();

    public void Update(double delta)
    {
        while (scripts.TryDequeue(out var script))
            script.Load();
    }

    internal void Enqueue(Scriptable script)
    {
        if (!scripts.Contains(script))
            scripts.Enqueue(script);
    }
}
