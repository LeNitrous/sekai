// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectIdentifierInfo
{
    public string Name { get; }
    public string Type { get; }

    public EffectIdentifierInfo(string name, string type)
    {
        Name = name;
        Type = type;
    }
}
