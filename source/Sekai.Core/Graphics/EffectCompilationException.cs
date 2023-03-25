// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// Exception thrown during effect compilation.
/// </summary>
public class EffectCompilationException : Exception
{
    public EffectCompilationException()
    {
    }

    public EffectCompilationException(string? message)
        : base(message)
    {
    }

    public EffectCompilationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
