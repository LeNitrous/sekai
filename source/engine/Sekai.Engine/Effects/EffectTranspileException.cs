// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Engine.Effects;

public class EffectTranspileException : Exception
{
    public EffectTranspileException(string? message)
        : base(message)
    {
    }
}
