// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Allocation;

/// <summary>
/// A container for registering or resolving cached services.
/// </summary>
public class ServiceContainer : FrameworkObject, IReadOnlyServiceContainer
{
    private readonly Dictionary<Type, Func<object>> cache;

    public ServiceContainer()
        : this(new Dictionary<Type, Func<object>>())
    {
    }

    private ServiceContainer(ServiceContainer other)
        : this(new Dictionary<Type, Func<object>>(other.cache))
    {
    }

    private ServiceContainer(Dictionary<Type, Func<object>> cache)
    {
        this.cache = cache;
    }

    /// <summary>
    /// Caches a service of a given type transiently.
    /// </summary>
    /// <param name="type">The object's type to be cached.</param>
    /// <param name="func">The function creating the service.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the type is already registered in this service container.</exception>
    public virtual void Cache(Type type, Func<object> func)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Services));

        if (cache.ContainsKey(type))
            throw new InvalidOperationException();

        cache.Add(type, func);
    }

    /// <summary>
    /// Caches a service as a singleton.
    /// </summary>
    /// <param name="instance">The service to cache.</param>
    /// <exception cref="ArgumentNullException">Thrown when the instance is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the type is already registered in this service container.</exception>
    public void Cache(object instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        Cache(instance.GetType(), () => instance);
    }

    /// <summary>
    /// Caches a service transiently.
    /// </summary>
    /// <typeparam name="T">The service's type to be cached.</typeparam>
    /// <param name="func">The function creating the service.</param>
    public void Cache<T>(Func<T> func)
    {
        Cache(typeof(T), () => func()!);
    }

    /// <summary>
    /// Caches a service of a given type as a singleton.
    /// </summary>
    /// <typeparam name="T">The service's type to be cached.</typeparam>
    /// <param name="instance">The service to be cached.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the type is already registered in this service container.</exception>
    public void Cache<T>(T instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        Cache(typeof(T), () => instance);
    }

    /// <summary>
    /// Caches a service of a given type as a singleton.
    /// </summary>
    /// <typeparam name="T">The service's type to be cached.</typeparam>
    /// <exception cref="ObjectDisposedException">Thrown when the service container is already disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the type is already registered in this service container.</exception>
    public void Cache<T>()
        where T : new()
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
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Services));

        if (cache.TryGetValue(type, out var func))
            return func();

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
    {
        object? result = Resolve(typeof(T), required);
        return result is null ? default : (T)result;
    }

    protected override void Destroy() => cache.Clear();

    public class Global : ServiceContainer
    {
        private readonly List<Scoped> scopes = new();

        public override void Cache(Type type, Func<object> instance)
        {
            base.Cache(type, instance);

            foreach (var scope in scopes)
                scope.Cache(type, instance);
        }

        public ServiceContainer CreateScoped()
        {
            var scoped = new Scoped(this);
            scopes.Add(scoped);
            return scoped;
        }

        private void remove(Scoped services) => scopes.Remove(services);

        private class Scoped : ServiceContainer
        {
            private readonly Global global;

            public Scoped(Global global)
            {
                this.global = global;
            }

            protected override void Destroy()
            {
                global.remove(this);
                base.Destroy();
            }
        }
    }
}
