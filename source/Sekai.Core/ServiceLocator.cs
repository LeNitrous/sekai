// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sekai;

/// <summary>
/// A container for services.
/// </summary>
public class ServiceLocator : IServiceLocator
{
    private readonly Dictionary<Type, object> services = new();

    /// <summary>
    /// Adds a service to this locator.
    /// </summary>
    /// <param name="type">The type of service.</param>
    /// <param name="instance">The service instance.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is already added or is not a class.</exception>
    /// <exception cref="InvalidCastException">Thrown when <paramref name="instance"/> cannot be assigned to <paramref name="type"/>.</exception>
    public void Add(Type type, object instance)
    {
        if (services.ContainsKey(type))
        {
            throw new ArgumentException($"{type} is already added to this locator.", nameof(type));
        }

        if (!type.IsClass)
        {
            throw new ArgumentException($"{type} must be a class.", nameof(type));
        }

        if (!instance.GetType().IsAssignableTo(type))
        {
            throw new InvalidCastException($"The {nameof(instance)} cannot be casted to {type}.");
        }

        services.Add(type, instance);
    }

    /// <summary>
    /// Removes a service from this locator.
    /// </summary>
    /// <param name="type">The service type to remove.</param>
    /// <returns><c>true</c> when the service is removed. <c>false</c> otherwise.</returns>
    public bool Remove(Type type)
    {
        return services.Remove(type);
    }

    /// <summary>
    /// Adds a service to this locator.
    /// </summary>
    /// <param name="instance">The service instance.</param>
    /// <typeparam name="T">The type of service.</typeparam>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is already added or is not a class.</exception>
    /// <exception cref="InvalidCastException">Thrown when <paramref name="instance"/> cannot be assigned to <typeparamref name="T"/>.</exception>
    public void Add<T>(T instance)
        where T : class
    {
        Add(typeof(T), instance);
    }

    /// <summary>
    /// Removes a service from this locator.
    /// </summary>
    /// <typeparam name="T">The service type to remove.</typeparam>
    /// <returns><c>true</c> when the service is removed. <c>false</c> otherwise.</returns>
    public bool Remove<T>()
        where T : class
    {
        return Remove(typeof(T));
    }

    public object? Get(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
        if (!services.TryGetValue(type, out object? service) && required)
        {
            throw new ServiceNotFoundException(type);
        }

        return service;
    }

    public T? Get<T>([DoesNotReturnIf(true)] bool required = true)
        where T : class
    {
        return (T?)Get(typeof(T), required);
    }
}
