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
    /// The name of this effect.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The effect source code.
    /// </summary>
    public string Code { get; set; }

    public EffectSource(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public override bool Equals(object? obj)
    {
        return obj is EffectSource other && Equals(other);
    }

    public bool Equals(EffectSource other)
    {
        return Name == other.Name &&
               Code == other.Code;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Code);
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
