// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Sekai.Framework.Annotations;
using Sekai.Framework.Extensions;

namespace Sekai.Framework.Allocation;

public partial class LoadableObject
{
    private static readonly Dictionary<Type, LoadableMetadata> metadata = new();

    internal class LoadableMetadata
    {
        private readonly Type type;
        private readonly bool cacheSelf;
        private readonly Type cacheAsType;
        private readonly Dictionary<Type, LoadableGetterEntry> getters = new();
        private readonly Dictionary<Type, LoadableSetterEntry> setters = new();

        public LoadableMetadata(Type type)
        {
            this.type = type;

            var attrib = type.GetCustomAttribute<CachedAttribute>();
            cacheSelf = attrib != null;
            cacheAsType = attrib?.AsType ?? type;

            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (member is MethodBase)
                    continue;

                register(member);
            }
        }

        public void Load(LoadableObject target)
        {
            if (cacheSelf)
                target.Container.Cache(cacheAsType, target);

            foreach ((var type, (var typeOverride, var func)) in getters)
            {
                object? value = func?.Invoke(target);
                target.Container.Cache(typeOverride ?? type, value!);
            }

            foreach ((var type, (bool required, var action)) in setters)
            {
                object? value = target.Container.Resolve(type, required);
                action?.Invoke(target, value!);
            }
        }

        private void register(MemberInfo member)
        {
            var attribCached = member.GetCustomAttribute<CachedAttribute>();
            var attribResolved = member.GetCustomAttribute<ResolvedAttribute>();
            bool isCacheable = attribCached != null;
            bool isResolvable = attribResolved != null;

            if (isResolvable && isCacheable)
                throw new InvalidOperationException(@$"Member cannot have both {nameof(ResolvedAttribute)} and {nameof(CachedAttribute)}.");

            if (!isResolvable && !isCacheable)
                return;

            var target = Expression.Parameter(typeof(object), "Target");
            var result = Expression.Parameter(typeof(object), "Result");

            bool isWritable = false;
            bool isReadable = false;
            bool isNullable = false;
            Type memberType = null!;
            MemberExpression memberAccess = null!;

            switch (member)
            {
                case PropertyInfo pi:
                    {
                        isReadable = pi.CanRead;
                        isWritable = pi.CanWrite;
                        isNullable = pi.IsNullable();
                        memberType = pi.PropertyType.GetUnderlyingNullableType() ?? pi.PropertyType;
                        memberAccess = Expression.Property(Expression.Convert(target, type), pi);
                        break;
                    }

                case FieldInfo fi:
                    {
                        isReadable = true;
                        isWritable = !fi.IsInitOnly;
                        isNullable = fi.IsNullable();
                        memberType = fi.FieldType.GetUnderlyingNullableType() ?? fi.FieldType;
                        memberAccess = Expression.Field(Expression.Convert(target, type), fi);
                        break;
                    }

                default:
                    return;
            }

            if (isResolvable)
            {
                if (!isWritable)
                    throw new InvalidOperationException(@"Cannot resolve on a read-only member.");

                var body = Expression.Assign(memberAccess, Expression.Convert(result, memberType));
                setters.Add(memberType, new LoadableSetterEntry(!isNullable, Expression.Lambda<Action<object, object>>(body, target, result).Compile()));
            }

            if (isCacheable)
            {
                if (!isReadable)
                    throw new InvalidOperationException(@"Cannot cache on write-only members.");

                var body = Expression.Convert(memberAccess, memberType);
                var cacheAsType = attribCached?.AsType ?? memberType;
                getters.Add(memberType, new LoadableGetterEntry(cacheAsType, Expression.Lambda<Func<object, object>>(body, target).Compile()));
            }
        }
    }

    private record struct LoadableGetterEntry(Type Type, Func<object, object> GetterFunc);
    private record struct LoadableSetterEntry(bool Required, Action<object, object> SetterFunc);
}
