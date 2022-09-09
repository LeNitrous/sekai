// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectDocumentPass
{
    public string Name { get; }
    public EffectMethodInfo[] Methods { get; }
    public EffectStructInfo[] Structs { get; }
    public EffectMemberInfo[] Members { get; }
    public EffectDirectiveInfo[] Directives { get; }

    public EffectDocumentPass(string name, EffectMethodInfo[] methods, EffectStructInfo[] structs, EffectMemberInfo[] members, EffectDirectiveInfo[] directives)
    {
        Name = name;
        Methods = methods;
        Structs = structs;
        Members = members;
        Directives = directives;
    }
}
