// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Compiler;

public struct EffectParameterInfo
{
    public string Name { get; }
    public string Type { get; }
    public int Size { get; }
    public EffectParameterFlags Flags { get; }

    public EffectParameterInfo(string name, string type, int size, EffectParameterFlags flags)
    {
        Name = name;
        Type = type;
        Size = size;
        Flags = flags;
    }
}
