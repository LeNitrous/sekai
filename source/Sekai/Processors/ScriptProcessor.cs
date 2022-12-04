// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Processors;

public sealed class ScriptProcessor : Processor<Script>
{
    private readonly Queue<Script> scripts = new();

    protected override void Process(double delta)
    {
        while (scripts.TryDequeue(out var script))
            script.Load();
    }

    protected override void Attach(Script script) => scripts.Enqueue(script);
}
