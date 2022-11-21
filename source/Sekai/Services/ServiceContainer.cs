// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Services;

public class ServiceContainer : FrameworkObject
{
    private readonly object syncLock = new();
    private readonly List<IGameService> services = new();
    private readonly Dictionary<Type, object> cache = new();

    /// <summary>
    /// Registers a given singleton instance as the given type.
    /// </summary>
    public void Register(Type type, object instance)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        if (cache.ContainsKey(type))
            throw new InvalidOperationException($"{type} is already cached.");

        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        var instanceType = instance.GetType();

        if (!instanceType.IsAssignableTo(type))
            throw new InvalidCastException($"Cannot cast {type} to {instanceType}");

        lock (syncLock)
        {
            cache.Add(type, instance);

            if (instance is IGameService or IDrawingGameService)
            {
                services.Add((IGameService)instance);
            }
        }
    }

    /// <inheritdoc cref="Register(Type, object)"/>
    public void Register<T>(T instance)
    {
        Register(typeof(T), instance!);
    }

    /// <inheritdoc cref="Register(Type, object)"/>
    public void Register<T>()
        where T : new()
    {
        Register(Activator.CreateInstance<T>());
    }

    /// <summary>
    /// Resolves an instance of the given type.
    /// </summary>
    public object Resolve(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        lock (syncLock)
        {
            if (cache.TryGetValue(type, out object? result))
            {
                var resultType = result.GetType();

                if (!resultType.IsAssignableTo(type))
                    throw new InvalidCastException($"Cannot cast {resultType} to {type}");

                return result;
            }
        }

        if (required)
            throw new KeyNotFoundException($"{type} is not registered in this container.");

        return null!;
    }

    /// <inheritdoc cref="Resolve(Type, bool)">
    public T Resolve<T>([DoesNotReturnIf(true)] bool required = true)
    {
        object? result = Resolve(typeof(T), required);
        return result is null ? default! : (T)result;
    }

    protected override void Destroy()
    {
        cache.Clear();
        services.Clear();
    }

    internal void Update(double delta)
    {
        for (int i = 0; i < services.Count; i++)
        {
            services[i]?.Update(delta);
        }
    }

    internal void FixedUpdate()
    {
        for (int i = 0; i < services.Count; i++)
        {
            services[i]?.FixedUpdate();
        }
    }

    internal void Render()
    {
        for (int i = 0; i < services.Count; i++)
        {
            services[i]?.Render();
        }
    }
}
