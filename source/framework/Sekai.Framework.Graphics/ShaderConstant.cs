// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct ShaderConstant : IEquatable<ShaderConstant>
{
    /// <summary>
    /// The constant variable ID as defined in the <see cref="IShader"/>.
    /// </summary>
    public uint ID;

    /// <summary>
    /// The type of data stored in this constant.
    /// </summary>
    public ShaderConstantType Type;

    /// <summary>
    /// An 8-byte block storing the contents of the specialization value.
    /// </summary>
    public ulong Data;

    public ShaderConstant(uint iD, ShaderConstantType type, ulong data)
    {
        ID = iD;
        Type = type;
        Data = data;
    }

    public override bool Equals(object? obj)
    {
        return obj is ShaderConstant constant && Equals(constant);
    }

    public bool Equals(ShaderConstant other)
    {
        return ID == other.ID &&
               Type == other.Type &&
               Data == other.Data;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Type, Data);
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
