// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectStructInfo
{
    public string Name { get; }
    public EffectStructMemberInfo[] Members { get; }

    public EffectStructInfo(string name, EffectStructMemberInfo[] members)
    {
        Name = name;
        Members = members;
    }
}
