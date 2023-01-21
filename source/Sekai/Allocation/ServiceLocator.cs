// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sekai.Allocation;

public class ServiceLocator : FrameworkObject
{
    /// <summary>
    /// The current active instance of the service locator.
    /// </summary>
    internal static ServiceLocator Current => current ?? throw new InvalidOperationException(@"The service locator is currently not initialized.");

    private static ServiceLocator? current;

    /// <summary>
    /// Initializes the service locator.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to initialize a new service locator.</exception>
    internal static void Initialize()
    {
        if (current is not null)
            throw new InvalidOperationException(@"An instance is currently initialized.");

        current = new();
    }

    /// <summary>
    /// Shuts down the service locator.
    /// </summary>
    internal static void Shutdown()
    {
        current?.Dispose();
        current = null;
    }

    private readonly Dictionary<Type, object> cached;

    private ServiceLocator()
    {
        cached = new();
    }

    private void cache(Type type, object instance)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        ArgumentNullException.ThrowIfNull(instance);

        lock (cached)
        {
            if (cached.ContainsKey(type))
                throw new ServiceExistsException($"{type} is already registered in this service container.");

            cached.Add(type, instance);
        }
    }

    /// <summary>
    /// Caches a service.
    /// </summary>
    /// <param name="instance">The service to cache.</param>
    /// <exception cref="ArgumentNullException">Thrown when the instance is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="ServiceExistsException">Thrown when the type is already registered in this service container.</exception>
    public void Cache(object instance) => cache(instance.GetType(), instance);

    /// <summary>
    /// Caches a service of a given type as a singleton.
    /// </summary>
    /// <typeparam name="T">The service's type to be cached.</typeparam>
    /// <param name="instance">The service to be cached.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="ServiceExistsException">Thrown when the type is already registered in this service container.</exception>
    public void Cache<T>(T instance)
        where T : class
    {
        cache(typeof(T), instance);
    }

    /// <summary>
    /// Caches a service of a given type as a singleton.
    /// </summary>
    /// <typeparam name="T">The service's type to be cached.</typeparam>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="ServiceExistsException">Thrown when the type is already registered in this service container.</exception>
    public void Cache<T>()
        where T : class, new()
    {
        Cache(Activator.CreateInstance<T>());
    }

    /// <summary>
    ///Resolves a service of a given type.
    /// </summary>
    /// <param name="type">The service's type to resolve.</param>
    /// <param name="required">>Whether to throw or return null if not found.</param>
    /// <returns>The resolved service or null if not found when not required.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="ServiceNotFoundException">Thrown when the service of a given type is not found.</exception>
    public object? Resolve(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        lock (cached)
        {
            if (cached.TryGetValue(type, out object? obj))
                return obj;
        }

        if (required)
            throw new ServiceNotFoundException($"A service with the type {type} was not found.");

        return null;
    }

    /// <summary>
    /// Resolves a service of a given type.
    /// </summary>
    /// <typeparam name="T">The service's type to resolve.</typeparam>
    /// <param name="required">Whether to throw or return null if not found.</param>
    /// <returns>The resolved service or null if not found when not required.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="ServiceNotFoundException">Thrown when the service of a given type is not found.</exception>
    public T? Resolve<T>([DoesNotReturnIf(true)] bool required = true)
        where T : class
    {
        return Unsafe.As<T>(Resolve(typeof(T), required));
    }

    protected override void Destroy()
    {
        lock (cached)
        {
            foreach (object obj in cached.Values.Reverse())
            {
                if (obj is IDisposable disposable)
                    disposable.Dispose();
            }

            cached.Clear();
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
