// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectMemberInfo
{
    public string Name { get; }
    public string Type { get; }
    public string Size { get; }
    public string Qualifier { get; }

    public EffectMemberInfo(string name, string type, string size, string qualifier)
    {
        Name = name;
        Type = type;
        Size = size;
        Qualifier = qualifier;
    }
}
