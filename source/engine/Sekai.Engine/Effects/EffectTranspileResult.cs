// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Engine.Effects;

public struct EffectTranspileResult
{
    /// <summary>
    /// The transpiled code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// The defined stages.
    /// </summary>
    public IReadOnlyList<EffectMember> Stages { get; }

    /// <summary>
    /// The defined resources.
    /// </summary>
    public IReadOnlyList<EffectMember> Resources { get; }

    public EffectTranspileResult(string code, IReadOnlyList<EffectMember> stages, IReadOnlyList<EffectMember> resources)
    {
        Code = code;
        Stages = stages;
        Resources = resources;
    }
}
