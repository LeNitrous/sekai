// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Sekai.Framework.Extensions;
using Sekai.Framework.Services;

namespace Sekai.Framework;

public partial class LoadableObject
{
    /// <summary>
    /// Stores information about a given type that extends <see cref="LoadableObject"/>.
    /// Used for injecting services during <see cref="LoadInternal"/>.
    /// </summary>
    private class LoadableMetadata
    {
        private readonly bool cacheSelfOnInit;
        private readonly Type? cacheAsType;
        private readonly Type type;
        private readonly Dictionary<Type, (Type?, Func<object, object>)> getters = new();
        private readonly Dictionary<Type, (bool, Action<object, object>)> setters = new();

        public LoadableMetadata(Type type)
        {
            this.type = type;

            var attrib = type.GetCustomAttribute<CachedAttribute>();
            cacheSelfOnInit = attrib != null;
            cacheAsType = attrib?.AsType;

            prepareGettersAndSetters();
        }

        public void Load(LoadableObject loadable)
        {
            if (cacheSelfOnInit)
                loadable.Services.Cache(cacheAsType ?? type, loadable);

            foreach ((var type, (bool req, var del)) in setters)
            {
                object? value = loadable.Services.Resolve(type, req);
                del?.Invoke(loadable, value!);
            }

            foreach ((var type, (var overrideType, var del)) in getters)
            {
                object? value = del?.Invoke(loadable);

                if (value is null)
                    throw new InvalidOperationException(@"Cannot cache a null value.");

                loadable.Services.Cache(overrideType ?? type, value);
            }
        }

        private void prepareGettersAndSetters()
        {
            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var resolvedAttrib = member.GetCustomAttribute<ResolvedAttribute>();
                var cachedAttrib = member.GetCustomAttribute<CachedAttribute>();
                bool resolvable = resolvedAttrib != null;
                bool cacheable = cachedAttrib != null;

                if (!resolvable && !cacheable)
                    continue;

                if (resolvable && cacheable)
                    throw new InvalidOperationException(@"Member cannot be resolvable and cacheable at the same time.");

                var target = Expression.Parameter(typeof(object), "Target");
                var result = Expression.Parameter(typeof(object), "Result");

                bool isWritable = false;
                bool isReadable = false;
                bool isNullable = false;
                Type? memberType = null;
                MemberExpression? memberAccess = null;

                if (member is PropertyInfo pi)
                {
                    isReadable = pi.CanRead;
                    isWritable = pi.CanWrite;
                    isNullable = pi.IsNullable();
                    memberType = pi.PropertyType.GetUnderlyingNullableType() ?? pi.PropertyType;
                    memberAccess = Expression.Property(Expression.Convert(target, type), pi);
                }

                if (member is FieldInfo fi)
                {
                    isReadable = true;
                    isWritable = !fi.IsInitOnly;
                    isNullable = fi.IsNullable();
                    memberType = fi.FieldType.GetUnderlyingNullableType() ?? fi.FieldType;
                    memberAccess = Expression.Field(Expression.Convert(target, type), fi);
                }

                if (cacheable)
                {
                    memberType = cachedAttrib!.AsType ?? memberType;
                }

                if (memberAccess is null || memberType is null)
                    throw new InvalidOperationException(@"Member not a field or property.");

                if (resolvable)
                {
                    if (!isWritable)
                        throw new InvalidOperationException(@"Cannot resolve on read-only members.");

                    var body = Expression.Assign(memberAccess, Expression.Convert(result, memberType));
                    setters.Add(memberType, (!isNullable, Expression.Lambda<Action<object, object>>(body, target, result).Compile()));
                    continue;
                }

                if (cacheable)
                {
                    if (!isReadable)
                        throw new InvalidOperationException(@"Cannot cache on write-only members.");

                    var body = Expression.Convert(memberAccess, memberType);
                    getters.Add(memberType, (cachedAttrib?.AsType, Expression.Lambda<Func<object, object>>(body, target).Compile()));
                    continue;
                }

                throw new InvalidOperationException($@"Failed to prepare metadata for ""{type.Name}.{member.Name}""");
            }
        }
    }
}
