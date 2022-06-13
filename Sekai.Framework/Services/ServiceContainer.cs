// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Services;

public class ServiceContainer : FrameworkObject, IReadOnlyServiceContainer
{
    public IReadOnlyDictionary<Type, Func<object>> Cached => cache;
    internal IReadOnlyServiceContainer? Parent;
    private readonly object syncLock = new();
    private readonly Dictionary<Type, Func<object>> cache = new();

    public ServiceContainer(IReadOnlyDictionary<Type, Func<object>>? cache = null)
    {
        if (cache == null)
            return;

        foreach ((var type, var func) in cache)
            this.cache.Add(type, func);
    }

    public T Resolve<T>([DoesNotReturnIf(true)] bool required = true)
    {
        object? result = Resolve(typeof(T), required);
        return result is null ? default! : (T)result;
    }

    public object Resolve(Type target, [DoesNotReturnIf(true)] bool required = true)
    {
        if (!cache.ContainsKey(target))
        {
            if (Parent != null)
                return Parent.Resolve(target, required);
        }

        lock (syncLock)
        {
            if (cache.TryGetValue(target, out var func))
            {
                object result = func();
                var type = result.GetType();

                if (!type.IsAssignableTo(target))
                    throw new InvalidCastException($"Cannot cast {type} to {target}");

                return result;
            }
        }

        if (required)
            throw new ServiceNotFoundException($"{target} is not currently registered.");

        return null!;
    }

    public void Cache(Type type, object instance)
    {
        if (!instance.GetType().IsAssignableTo(type))
            throw new InvalidCastException($"Cannot cast {type} to {instance.GetType()}");

        if (cache.ContainsKey(type))
            throw new ServiceExistsException($"{type} is already registered for this container.");

        lock (syncLock)
        {
            cache.Add(type, () => instance!);
        }
    }

    public void Cache(Type type, Func<object> creationFunc)
    {
        if (cache.ContainsKey(type))
            throw new ServiceExistsException($"{type} is already registered for this container.");

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

    protected override void Destroy()
    {
        Parent = null;
        cache.Clear();
    }
}

public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(string? message)
        : base(message)
    {
    }
}

public class ServiceExistsException : Exception
{
    public ServiceExistsException(string? message)
        : base(message)
    {
    }
}
