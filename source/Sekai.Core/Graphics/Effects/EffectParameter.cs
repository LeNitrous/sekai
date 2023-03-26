// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;

namespace Sekai.Graphics.Effects;

[DebuggerDisplay("\\{Name = {Name}\\}")]
internal abstract class EffectParameter
{
    public string Name { get; }

    protected EffectParameter(string name)
    {
        Name = name;
    }
}

[DebuggerDisplay("\\{Name = {Name}, Size = {Size}, Offset = {Offset}\\}")]
internal sealed class EffectValueParameter : EffectParameter
{
    public int Size { get; }

    public int Offset { get; }

    public EffectValueParameter(string name, int size, int offset)
        : base(name)
    {
        Size = size;
        Offset = offset;
    }
}

internal sealed class EffectOpaqueParameter : EffectParameter
{
    public EffectOpaqueParameter(string name)
        : base(name)
    {
    }
}

internal class EffectParameterNotFoundException : Exception
{
    public EffectParameterNotFoundException()
    {
    }

    public EffectParameterNotFoundException(string? message)
        : base(message)
    {
    }

    public EffectParameterNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
