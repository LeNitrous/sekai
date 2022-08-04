// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Containers;

public class Container : FrameworkObject, IContainer, IReadOnlyContainer
{
    private readonly object syncLock = new();
    private readonly IReadOnlyContainer parent = null!;
    private readonly Dictionary<Type, Func<object>> cache = new();

    public Container(IReadOnlyContainer? parent = null)
    {
        this.parent = parent!;
    }

    public void Cache(Type type, object instance)
    {
        if (instance is null)
            throw new NullReferenceException(@"Instance cannot be null.");

        var instanceType = instance.GetType();

        if (!instanceType.IsAssignableTo(type))
            throw new InvalidCastException($"Cannot cast {type} to {instanceType}");

        Cache(type, () => instance);
    }

    public void Cache(Type type, Func<object> creationFunc)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Container));

        if (creationFunc is null)
            throw new NullReferenceException(@"Function cannot be null.");

        if (cache.ContainsKey(type))
            throw new InvalidOperationException($"{type} is already cached.");

        lock (syncLock)
        {
            cache.Add(type, creationFunc);
        }
    }

    public void Cache<T>(T instance)
    {
        Cache(typeof(T), instance!);
    }

    public void Cache<T>(Func<T> creationFunc)
    {
        Cache(typeof(T), () => creationFunc()!);
    }

    public T Resolve<T>([DoesNotReturnIf(true)] bool required = true)
    {
        object? result = Resolve(typeof(T), required);
        return result is null ? default! : (T)result;
    }

    public object Resolve(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Container));

        lock (syncLock)
        {
            if (cache.TryGetValue(type, out var func))
            {
                object result = func();
                var resultType = result.GetType();

                if (!resultType.IsAssignableTo(type))
                    throw new InvalidCastException($"Cannot cast {resultType} to {type}");

                return result;
            }
        }

        if (parent != null)
        {
            object result = parent.Resolve(type, false);

            if (result != null)
                return result;
        }

        if (required)
            throw new KeyNotFoundException($"{type} is not registered in this contianer.");

        return null!;
    }

    protected override void Destroy()
    {
        cache.Clear();
    }
}
