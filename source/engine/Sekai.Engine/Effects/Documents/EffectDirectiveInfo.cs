// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Effects.Documents;

public struct EffectDirectiveInfo
{
    public string Key { get; }
    public string Value { get; }

    public EffectDirectiveInfo(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
