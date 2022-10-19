// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Engine.Effects;
using Sekai.Framework;

namespace Sekai.Engine.Rendering;

public class Material : FrameworkObject
{
    /// <summary>
    /// Retrieves a material pass of the given name.
    /// </summary>
    public MaterialPass this[string name] => passes[name];

    /// <summary>
    /// Gets the effect used by this material.
    /// </summary>
    public Effect Effect { get; }

    private readonly Dictionary<string, MaterialPass> passes = new();

    internal Material(RenderContext context, Effect effect)
    {
        Effect = effect;

        foreach (var pass in effect.Passes)
            passes.Add(pass.Name, new MaterialPass(context, effect, pass));
    }

    protected override void Destroy()
    {
        foreach (var pass in passes.Values)
            pass.Dispose();
    }
}
