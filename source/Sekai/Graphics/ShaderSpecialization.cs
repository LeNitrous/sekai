// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sekai.Graphics;

/// <summary>
/// Values injected halfway during compilation of a <see cref="Shader"/>.
/// </summary>
public readonly struct ShaderConstant : IEquatable<ShaderConstant>
{
    /// <summary>
    /// The constant's ID.
    /// </summary>
    public readonly uint ID;

    /// <summary>
    /// The constant's value.
    /// </summary>
    public readonly ulong Value;

    /// <summary>
    /// The constant's type.
    /// </summary>
    public readonly ShaderConstantType Type;

    private ShaderConstant(uint id, ulong value, ShaderConstantType type)
    {
        ID = id;
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Creates a new <see cref="ShaderConstant"/>.
    /// </summary>
    /// <typeparam name="T">The constant type.</typeparam>
    /// <param name="id">The ID of the constant.</param>
    /// <param name="value">The value of the constant.</param>
    /// <returns>A new constant.</returns>
    public static unsafe ShaderConstant Create<T>(uint id, T value)
        where T : unmanaged, INumber<T>
    {
        ulong v;
        Unsafe.Write(&v, value);

        var type = value switch
        {
            bool => ShaderConstantType.Bool,
            ushort => ShaderConstantType.UInt16,
            short => ShaderConstantType.Int16,
            uint => ShaderConstantType.UInt32,
            int => ShaderConstantType.Int32,
            ulong => ShaderConstantType.UInt64,
            long => ShaderConstantType.Int64,
            float => ShaderConstantType.Float,
            double => ShaderConstantType.Double,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };

        return new(id, v, type);
    }

    public bool Equals(ShaderConstant other)
    {
        return ID == other.ID && Value == other.Value && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        return obj is ShaderConstant other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Value, Type);
    }

    public static bool operator ==(ShaderConstant left, ShaderConstant right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ShaderConstant left, ShaderConstant right)
    {
        return !(left == right);
    }
}
