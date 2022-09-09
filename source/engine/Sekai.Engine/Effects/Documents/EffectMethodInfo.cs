// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectMethodInfo
{
    public string Name { get; }
    public string Type { get; }
    public string Body { get; }
    public EffectIdentifierInfo[] Parameters { get; }

    public EffectMethodInfo(string name, string type, string body, EffectIdentifierInfo[] parameters)
    {
        Name = name;
        Type = type;
        Body = body;
        Parameters = parameters;
    }
}
