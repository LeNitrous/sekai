// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Analysis;

public struct EffectAnalysisResult
{
    public EffectStageInfo[] Stages { get; }
    public EffectParameterInfo[] Parameters { get; }

    public EffectAnalysisResult(EffectStageInfo[] stages, EffectParameterInfo[] parameters)
    {
        Stages = stages;
        Parameters = parameters;
    }
}
