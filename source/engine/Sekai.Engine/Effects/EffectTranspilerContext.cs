// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Engine.Effects;

public struct EffectTranspilerContext
{
    /// <summary>
    /// The defined stages.
    /// </summary>
    public IReadOnlyList<EffectMember> Stages { get; }

    /// <summary>
    /// The defined resources.
    /// </summary>
    public IReadOnlyList<EffectMember> Resources { get; }

    public EffectTranspilerContext(IReadOnlyList<EffectMember> stages, IReadOnlyList<EffectMember> resources)
    {
        Stages = stages;
        Resources = resources;
    }
}
