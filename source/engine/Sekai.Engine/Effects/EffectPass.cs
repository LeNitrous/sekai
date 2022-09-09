// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Engine.Effects.Compiler;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Effects;

public class EffectPass : FrameworkObject
{
    /// <summary>
    /// The effect pass name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The declared vertex inputs for this effect.
    /// </summary>
    public IReadOnlyList<EffectStageInfo> Stages { get; }

    /// <summary>
    /// The parameters for this effect.
    /// </summary>
    public IReadOnlyList<EffectParameterInfo> Parameters { get; }

    /// <summary>
    /// The compiled shaders for this pass.
    /// </summary>
    internal IShader[] Shaders { get; }

    /// <summary>
    /// The resource layout used by the compiled shaders.
    /// </summary>
    internal IResourceLayout Layout { get; }

    internal EffectPass(string name, IReadOnlyList<EffectStageInfo> stages, IReadOnlyList<EffectParameterInfo> parameters, IResourceLayout layout, IShader[] shaders)
    {
        Name = name;
        Stages = stages;
        Layout = layout;
        Shaders = shaders;
        Parameters = parameters;
    }

    protected override void Destroy()
    {
        for (int i = 0; i < Shaders.Length; i++)
            Shaders[i]?.Dispose();

        Layout?.Dispose();
    }
}
