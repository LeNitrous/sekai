// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Allocation;

/// <summary>
/// An interface providing read-only access to services.
/// </summary>
public interface IReadOnlyServiceContainer : IDisposable
{
    /// <summary>
    /// Resolves a given type from the service registry.
    /// </summary>
    object? Resolve(Type type, [DoesNotReturnIf(true)] bool required = true);

    /// <summary>
    /// Resolves a given type from the service registry.
    /// </summary>
    T? Resolve<T>([DoesNotReturnIf(true)] bool required = true);
}
