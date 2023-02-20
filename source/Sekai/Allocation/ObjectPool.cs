// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;

namespace Sekai.Allocation;

/// <summary>
/// A collection of objects that can contain a certain amount but can never exceed its capacity allowing reusal of objects.
/// </summary>
/// <typeparam name="T">The object type to pool.</typeparam>
public class ObjectPool<T> : DisposableObject
    where T : notnull
{
    private readonly int capacity;
    private readonly ConcurrentBag<T> items = new();
    private readonly ObjectPoolingStrategy<T> strategy;

    /// <summary>
    /// Creates an object pool
    /// </summary>
    /// <param name="capacity">The maximum capacity of the pool.</param>
    /// <param name="strategy">The pooling strategy of the pool.</param>
    public ObjectPool(int capacity, ObjectPoolingStrategy<T> strategy)
    {
        this.capacity = capacity;
        this.strategy = strategy;
    }

    /// <summary>
    /// Gets an object from the pool.
    /// </summary>
    public T Get()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ObjectPool<T>));

        if (capacity > items.Count)
        {
            if (items.TryTake(out var item))
                return item;
        }

        return strategy.Create();
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    /// <param name="obj">The object to return.</param>
    public void Return(T obj)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ObjectPool<T>));

        if (capacity < items.Count || !strategy.Return(obj))
        {
            if (obj is IDisposable disposable)
                disposable.Dispose();

            return;
        }

        items.Add(obj);
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var item in items)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
