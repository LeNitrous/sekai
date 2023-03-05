// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai;

/// <summary>
/// An interface for objects capable of locating services.
/// </summary>
public interface IServiceLocator
{
    /// <summary>
    /// Gets the service of a given type.
    /// </summary>
    /// <param name="type">The object type to resolve.</param>
    /// <param name="required">Whether the service is required or not.</param>
    /// <returns>The service object of the given type or <c>null</c> when <paramref name="required"/> is <c>false</c> and the service is not found.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when <paramref name="required"/> is <c>true</c> and the service is not found.</exception>
    object? Get(Type type, [DoesNotReturnIf(true)] bool required = true);

    /// <summary>
    /// Gets the service of a given type.
    /// </summary>
    /// <typeparam name="T">The object type to resolve.</typeparam>
    /// <param name="required">Whether the service is required or not.</param>
    /// <returns>The service object of type <typeparamref name="T"/> or <c>null</c> when <paramref name="required"/> is <c>false</c> and the service is not found.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when <paramref name="required"/> is <c>true</c> and the service is not found.</exception>
    T? Get<T>([DoesNotReturnIf(true)] bool required = true) where T : class;
}
