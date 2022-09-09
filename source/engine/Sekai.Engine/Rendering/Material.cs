// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Engine.Effects;
using Sekai.Framework;

namespace Sekai.Engine.Rendering;

public class Material : FrameworkObject
{
    public MaterialPass this[string name] => passes[name];
    private readonly Dictionary<string, MaterialPass> passes = new();

    /// <summary>
    /// Gets the effect used by this material.
    /// </summary>
    public Effect Effect { get; }

    public Material(Effect effect)
    {
        Effect = effect;

        foreach (var pass in effect.Passes)
            passes.Add(pass.Name, new MaterialPass(effect, pass));
    }

    protected override void Destroy()
    {
        foreach (var pass in passes.Values)
            pass.Dispose();
    }
}
