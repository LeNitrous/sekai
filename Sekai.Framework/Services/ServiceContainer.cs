// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Services;

public class ServiceContainer : IReadOnlyServiceContainer
{
    private readonly IReadOnlyServiceContainer? parent;
    private readonly Dictionary<Type, Func<object>> transientCache = new();
    private readonly Dictionary<Type, object> singletonCache = new();
    private readonly List<Type> registeredTypes = new();
    private readonly object syncLock = new();

    public ServiceContainer(IReadOnlyServiceContainer? parent = null)
    {
        this.parent = parent;
    }

    public T? Resolve<T>([DoesNotReturnIf(true)] bool required = false)
    {
        if (!registeredTypes.Contains(typeof(T)))
        {
            if (parent != null)
                return parent.Resolve<T>(required);
        }

        lock (syncLock)
        {
            if (singletonCache.TryGetValue(typeof(T), out var value))
            {
                var type = value.GetType();

                if (!type.IsAssignableTo(typeof(T)))
                    throw new InvalidCastException($"Cannot cast {type} to {typeof(T)}");

                return (T)value;
            }

            if (transientCache.TryGetValue(typeof(T), out var func))
            {
                var result = func();
                var type = result.GetType();

                if (!type.IsAssignableTo(typeof(T)))
                    throw new InvalidCastException($"Cannot cast {type} to {typeof(T)}");

                return (T)func();
            }
        }

        if (required)
            throw new ServiceNotFoundException($"{typeof(T)} is not currently registered.");

        return default;
    }

    public void Cache<T>(T instance)
    {
        if (registeredTypes.Contains(typeof(T)))
            throw new ServiceExistsException($"{typeof(T)} is already registered for this container.");

        lock (syncLock)
        {
            registeredTypes.Add(typeof(T));
            singletonCache.Add(typeof(T), instance!);
        }
    }

    public void Cache<T>(Func<T> creationFunc)
    {
        if (registeredTypes.Contains(typeof(T)))
            throw new ServiceExistsException($"{typeof(T)} is already registered for this container.");

        lock (syncLock)
        {
            registeredTypes.Add(typeof(T));
            transientCache.Add(typeof(T), () => creationFunc()!);
        }
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
