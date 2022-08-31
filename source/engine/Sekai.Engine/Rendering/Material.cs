// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Engine.Effects;
using Sekai.Framework;

namespace Sekai.Engine.Rendering;

public class Material : FrameworkObject
{
    public MaterialPass this[string name] => passes[name];
    private readonly Dictionary<string, MaterialPass> passes = new();

    public void AddPass(string name, Effect effect)
    {
        if (passes.ContainsKey(name))
            throw new InvalidOperationException();

        passes.Add(name, new MaterialPass(effect));
    }

    public void RemovePass(string name)
    {
        if (!passes.Remove(name))
            throw new InvalidOperationException();
    }
}
