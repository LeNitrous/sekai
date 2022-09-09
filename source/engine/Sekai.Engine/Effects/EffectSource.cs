// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Engine.Effects;

/// <summary>
/// Represents uncompiled shader code.
/// </summary>
public struct EffectSource : IEquatable<EffectSource>
{
    /// <summary>
    /// The effect source code.
    /// </summary>
    public string Code { get; set; }

    public EffectSource(string code)
    {
        Code = code;
    }

    public override bool Equals(object? obj)
    {
        return obj is EffectSource other && Equals(other);
    }

    public bool Equals(EffectSource other)
    {
        return Code == other.Code;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code);
    }

    public static bool operator ==(EffectSource left, EffectSource right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EffectSource left, EffectSource right)
    {
        return !(left == right);
    }
}
