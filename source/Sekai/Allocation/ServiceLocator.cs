// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sekai.Allocation;

/// <summary>
/// Contains an immutable list of resolvable services.
/// </summary>
public sealed class ServiceLocator : DisposableObject
{
    private readonly IReadOnlyList<ServiceDescriptor> services;

    internal ServiceLocator(IReadOnlyList<ServiceDescriptor> services)
    {
        this.services = services;
    }

    /// <summary>
    /// Retrieves the contract of <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The service type to retrieve.</param>
    /// <param name="required">Whether the service is required to be not <see cref="null"/> when returned.</param>
    /// <returns>The resolved service.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when the service was not found and <paramref name="required"/> is true.</exception>
    /// <exception cref="InvalidOperationException">Thrown when there are multiple services of <paramref name="type"/> that were found.</exception>
    public ServiceContract? Locate(Type type, [DoesNotReturnIf(true)] bool required = true)
    {
        var resolved = LocateAll(type).SingleOrDefault();

        if (resolved is null && required)
            throw new ServiceNotFoundException(type);

        return resolved;
    }

    /// <inheritdoc cref="Locate(Type, bool)"/>
    public ServiceContract? Locate<T>([DoesNotReturnIf(true)] bool required = true)
        where T : class
    {
        return Locate(typeof(T), required)!;
    }

    /// <summary>
    /// Retrieves all contracts of <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The service type to retrieve.</param>
    /// <returns>An enumeration of service instances.</returns>
    public IEnumerable<ServiceContract> LocateAll(Type type)
    {
        return services.Where(d => d.Type.IsAssignableTo(type)).Select(static d => d.Resolve());
    }

    /// <inheritdoc cref="LocateAll(Type)"/>
    public IEnumerable<ServiceContract> LocateAll<T>()
        where T : class
    {
        return LocateAll(typeof(T));
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var service in services)
            service.Dispose();
    }
}

/// <summary>
/// Represents errors that occure when a service was not found.
/// </summary>
public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(Type type)
        : base($"The required service {type} was not found.")
    {
    }
}
