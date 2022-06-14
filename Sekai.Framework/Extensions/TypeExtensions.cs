// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Sekai.Framework.Extensions;

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, Type> underlying_type_cache = new();

    public static Type? GetUnderlyingNullableType(this Type type)
    {
        return underlying_type_cache.GetOrAdd(type, t => Nullable.GetUnderlyingType(t)!);
    }

    public static bool IsNullable(this Type type)
    {
        return GetUnderlyingNullableType(type) != null;
    }

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

    public static bool IsNullable(this FieldInfo fi)
    {
        if (IsNullable(fi.FieldType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(fi));
    }

    public static bool IsNullable(this PropertyInfo pi)
    {
        if (IsNullable(pi.PropertyType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(pi));
    }

    public static bool IsNullable(this ParameterInfo pi)
    {
        if (IsNullable(pi.ParameterType))
            return true;

        return isNullable(new NullabilityInfoContext().Create(pi));
    }

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
