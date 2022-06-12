// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Sekai.Framework.Extensions;
using Sekai.Framework.Services;

namespace Sekai.Framework;

public abstract class LoadableObject : FrameworkObject, ILoadableObjectContainer
{
    public bool IsLoaded { get; private set; }
    public readonly ServiceContainer Services = new();
    private LoadableObject? parent;
    private readonly List<LoadableObject> loadables = new();

    internal void Initialize()
    {
        if (IsLoaded)
            throw new InvalidOperationException(@"This loadable is already loaded.");

        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot load destroyed loadables.");

        var type = GetType();

        if (!metadatas.TryGetValue(type, out var metadata))
        {
            metadata = new LoadableData(type);
            metadatas.Add(type, metadata);
        }

        metadata.Load(this);

        foreach (var loadable in loadables)
            loadable.Initialize();

        IsLoaded = true;
        OnLoad();
    }

    protected virtual void OnLoad()
    {
    }

    protected virtual void OnUnload()
    {
    }

    protected override void Destroy()
    {
        IsLoaded = false;
        Services.Dispose();
        OnUnload();
    }

    IReadOnlyList<LoadableObject> ILoadableObjectContainer.Children => loadables;
    LoadableObject? ILoadableObjectContainer.Parent => parent;

    void ILoadableObjectContainer.Add(LoadableObject loadable)
    {
        lock (loadables)
        {
            if (loadables.Contains(loadable))
                return;

            loadable.parent = this;
            loadables.Add(loadable);
        }

        if (IsLoaded)
            loadable.Initialize();
    }

    void ILoadableObjectContainer.Remove(LoadableObject loadable)
    {
        lock (loadables)
        {
            loadable.parent = null;
            loadables.Remove(loadable);
        }
    }

    private static readonly Dictionary<Type, LoadableData> metadatas = new();

    private class LoadableData
    {
        private readonly Type type;
        private readonly Dictionary<Type, Func<object, object>> getters = new();
        private readonly Dictionary<Type, (bool, Action<object, object>)> setters = new();

        public LoadableData(Type type)
        {
            this.type = type;
            prepareGettersAndSetters();
        }

        public void Load(LoadableObject loadable)
        {
            foreach ((var type, (bool req, var del)) in setters)
            {
                object? value = loadable.Services.Resolve(type, req);
                del?.Invoke(loadable, value!);
            }

            foreach ((var type, var del) in getters)
            {
                object? value = del?.Invoke(loadable);

                if (value is null)
                    throw new InvalidOperationException(@"Cannot cache a null value.");

                loadable.Services.Cache(type, value);
            }
        }

        private void prepareGettersAndSetters()
        {
            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bool resolvable = member.GetCustomAttribute<ResolvedAttribute>() != null;
                bool cacheable = member.GetCustomAttribute<CachedAttribute>() != null;

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
                    memberType = pi.PropertyType.GetUnderlyingNullableType();
                    memberAccess = Expression.Property(Expression.Convert(target, type), pi);
                }

                if (member is FieldInfo fi)
                {
                    isReadable = true;
                    isWritable = !fi.IsInitOnly;
                    isNullable = fi.IsNullable();
                    memberType = fi.FieldType.GetUnderlyingNullableType();
                    memberAccess = Expression.Field(Expression.Convert(target, type), fi);
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
                    getters.Add(memberType, Expression.Lambda<Func<object, object>>(body, target).Compile());
                    continue;
                }

                throw new InvalidOperationException($@"Failed to prepare metadata for ""{type.Name}.{member.Name}""");
            }
        }
    }
}
