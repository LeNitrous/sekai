// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Allocation;

/// <summary>
/// Determines the strategy for an <see cref="ObjectPool{T}"/>.
/// </summary>
public abstract class ObjectPoolingStrategy<T> : FrameworkObject
    where T : notnull
{
    /// <summary>
    /// Creates an object.
    /// </summary>
    public abstract T Create();

    /// <summary>
    /// Returns an object.
    /// </summary>
    /// <param name="obj">The object to return.</param>
    /// <returns>True to keep the object. False to discard the object.</returns>
    public abstract bool Return(T obj);
}
