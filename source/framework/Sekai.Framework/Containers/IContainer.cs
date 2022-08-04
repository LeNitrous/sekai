// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Containers;

public interface IContainer
{
    /// <summary>
    /// Caches instance as a singleton.
    /// </summary>
    void Cache(Type type, object instance);

    /// <summary>
    /// Caches instance as transient.
    /// </summary>
    void Cache(Type type, Func<object> creationFunc);

    /// <summary>
    /// Caches instance as a singleton.
    /// </summary>
    void Cache<T>(T instance);

    /// <summary>
    /// Caches instance as transient.
    /// </summary>
    void Cache<T>(Func<T> creationFunc);

    /// <summary>
    /// Resolves instance of type.
    /// </summary>
    T Resolve<T>([DoesNotReturnIf(true)] bool required = true);

    /// <summary>
    /// Resolves instance of type.
    /// </summary>
    object Resolve(Type type, [DoesNotReturnIf(true)] bool required = true);
}
