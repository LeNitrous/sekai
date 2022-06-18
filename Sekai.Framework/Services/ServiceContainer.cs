// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Services;

public class ServiceContainer : FrameworkObject, IServiceContainer
{
    public IReadOnlyServiceContainer? Parent { get; set; }
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

    public object Resolve(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
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

        if (Parent != null)
            return Parent.Resolve(type, required);

        if (required)
            throw new ServiceNotFoundException(type);

        return null!;
    }

    public void Cache(Type type, object instance)
    {
        if (!instance.GetType().IsAssignableTo(type))
            throw new InvalidCastException($"Cannot cast {type} to {instance.GetType()}");

        if (cache.ContainsKey(type))
            throw new ServiceExistsException(type);

        lock (syncLock)
        {
            cache.Add(type, () => instance!);
        }
    }

    public void Cache(Type type, Func<object> creationFunc)
    {
        if (cache.ContainsKey(type))
            throw new ServiceExistsException(type);

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
        cache.Clear();
    }
}

public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(Type type)
        : base($"{type} is not currently registered.")
    {
    }
}

public class ServiceExistsException : Exception
{
    public ServiceExistsException(Type type)
        : base($"{type} is already registered for this container.")
    {
    }
}
