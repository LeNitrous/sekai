// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Analysis;

public struct EffectStageInfo
{
    public string Name { get; }
    public string Type { get; }

    public EffectStageInfo(string name, string type)
    {
        Name = name;
        Type = type;
    }
}
