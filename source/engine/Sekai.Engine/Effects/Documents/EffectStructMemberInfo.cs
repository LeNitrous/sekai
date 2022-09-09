// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectStructMemberInfo
{
    public string Name { get; }
    public string Type { get; }
    public string Size { get; }

    public EffectStructMemberInfo(string name, string type, string size)
    {
        Name = name;
        Type = type;
        Size = size;
    }
}
