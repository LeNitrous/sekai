// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a controller's control stick deadzone.
/// </summary>
public readonly struct Deadzone : IEquatable<Deadzone>
{
    /// <summary>
    /// The deadzone size.
    /// </summary>
    public float Value { get; }

    /// <summary>
    /// The deadzone method.
    /// </summary>
    public DeadzoneMethod Method { get; }

    /// <summary>
    /// Creates a new deadzone.
    /// </summary>
    /// <param name="value">The deadzone size.</param>
    /// <param name="method">The deadzone method.</param>
    public Deadzone(float value, DeadzoneMethod method)
    {
        Value = value;
        Method = method;
    }

    /// <summary>
    /// Applies the raw value to this deadzone.
    /// </summary>
    /// <param name="raw">The raw value to apply.</param>
    /// <returns>A new value based on this deadzone's rules.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid deadzone method was specified in the constructor.</exception>
    public float Apply(float raw) => Method switch
    {
        DeadzoneMethod.Traditional => Math.Abs(raw) < Value ? 0 : raw,
        DeadzoneMethod.AdaptiveGradient => ((1 - Value) * raw) + (Value * Math.Sign(raw)),
        _ => throw new InvalidOperationException("Invalid deadzone method specified."),
    };

    public bool Equals(Deadzone other)
    {
        return Value.Equals(other.Value) && Method == other.Method;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Deadzone deadzone && Equals(deadzone);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Method);
    }

    public static bool operator ==(Deadzone left, Deadzone right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Deadzone left, Deadzone right)
    {
        return !(left == right);
    }
}
