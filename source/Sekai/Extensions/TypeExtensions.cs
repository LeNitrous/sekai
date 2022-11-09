// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Sekai.Extensions;

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, Type> underlying_type_cache = new();

    /// <summary>
    /// Returns the underlying type under <see cref="Nullable{T}"/>
    /// </summary>
    /// <returns>The underlying type for value types or null for reference types.</returns>
    public static Type? GetUnderlyingNullableType(this Type type)
    {
        return underlying_type_cache.GetOrAdd(type, t => Nullable.GetUnderlyingType(t)!);
    }

    /// <summary>
    /// Returns whether a given type is a <see cref="Nullable{T}"/> or not.
    /// </summary>
    public static bool IsNullable(this Type type)
    {
        return GetUnderlyingNullableType(type) != null;
    }

    /// <summary>
    /// Returns whether the member's type is a nullable or not.
    /// </summary>
    public static bool IsNullable(this MemberInfo member)
    {
        if (member is FieldInfo fi)
            return IsNullable(fi);

        if (member is PropertyInfo pi)
            return IsNullable(pi);

        if (member is EventInfo e)
            return IsNullable(e);

        return false;
    }

    /// <summary>
    /// Returns whether the field is a nullable or not.
    /// </summary>
    public static bool IsNullable(this FieldInfo fi)
    {
        if (IsNullable(fi.FieldType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(fi));
    }

    /// <summary>
    /// Returns whether the property is a nullable or not.
    /// </summary>
    public static bool IsNullable(this PropertyInfo pi)
    {
        if (IsNullable(pi.PropertyType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(pi));
    }

    /// <summary>
    /// Returns whether the parameter is a nullable or not.
    /// </summary>
    public static bool IsNullable(this ParameterInfo pi)
    {
        if (IsNullable(pi.ParameterType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(pi));
    }

    /// <summary>
    /// Returns whether the event is a nullable or not.
    /// </summary>
    public static bool IsNullable(this EventInfo e)
    {
        if (e.EventHandlerType is null)
            return false;

        if (IsNullable(e.EventHandlerType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(e));
    }

    private static bool isNullable(NullabilityInfo info)
    {
        return info.WriteState == NullabilityState.Nullable || info.ReadState == NullabilityState.Nullable;
    }
}
