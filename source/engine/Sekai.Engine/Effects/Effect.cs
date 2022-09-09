// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Effects;

public class Effect : FrameworkObject
{
    /// <summary>
    /// The effect name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The effect type.
    /// </summary>
    public EffectType Type { get; }

    public EffectPass this[string pass] => passes[pass];

    public IEnumerable<EffectPass> Passes => passes.Values;

    internal IGraphicsDevice Device { get; }

    private readonly Dictionary<string, EffectPass> passes = new();

    internal Effect(string name, EffectType type, IGraphicsDevice device, IReadOnlyList<EffectPass> passes)
    {
        Name = name;
        Type = type;
        Device = device;

        foreach (var pass in passes)
            this.passes.Add(pass.Name, pass);
    }

    protected override void Destroy()
    {
        foreach (var pass in passes.Values)
            pass.Dispose();
    }
}
