// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Engine.Effects;

/// <summary>
/// Defines a global member in an effect.
/// </summary>
public class EffectMember
{
    /// <summary>
    /// The name of this member.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The GLSL type.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// The size of this member.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// The flags that define this member.
    /// </summary>
    public EffectMemberFlags Flags { get; }

    internal EffectMember(string name, string type, int size, EffectMemberFlags flags)
    {
        Name = name;
        Type = type;
        Size = size;
        Flags = flags;
    }

    internal static EffectMember Create(string name, string type, int size, EffectMemberFlags flags) => type switch
    {
        "int" => new EffectMember<int>(name, type, size, flags),
        "uint" => new EffectMember<uint>(name, type, size, flags),
        "bool" => new EffectMember<bool>(name, type, size, flags),
        "vec2" => new EffectMember<Vector2>(name, type, size, flags),
        "vec3" => new EffectMember<Vector3>(name, type, size, flags),
        "vec4" => new EffectMember<Vector4>(name, type, size, flags),
        "mat2" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat3" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat4" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "float" => new EffectMember<float>(name, type, size, flags),
        "mat2x2" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat2x3" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat2x4" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat3x2" => new EffectMember<Matrix3x2>(name, type, size, flags),
        "mat3x3" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat3x4" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat4x2" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat4x3" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "mat4x4" => new EffectMember<Matrix4x4>(name, type, size, flags),
        "double" => new EffectMember<double>(name, type, size, flags),
        _ => new EffectMember(name, type, size, flags),
    };
}

/// <summary>
/// Defines a strongly typed global member in an effect.
/// </summary>
public class EffectMember<T> : EffectMember
    where T : struct
{
    internal EffectMember(string name, string type, int size, EffectMemberFlags flags)
        : base(name, type, size, flags)
    {
    }
}
