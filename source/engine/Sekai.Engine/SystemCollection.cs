// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Allocation;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

[Cached]
public class SystemCollection<T> : LoadableObject, IReadOnlyList<T>, IEnumerable<T>
    where T : LoadableObject
{
    public int Count => systems.Count;

    public T this[int index] => systems[index];

    private readonly List<T> systems = new();

    public U Get<U>()
        where U : T, new()
    {
        var instances = systems.OfType<U>();

        if (!instances.Any())
            throw new KeyNotFoundException($"Game system \"{typeof(T)}\" is not registered.");

        return instances.Single();
    }

    public void Register<U>()
        where U : T, new()
    {
        if (systems.OfType<U>().Any())
            throw new InvalidOperationException($"Game system \"{typeof(T)}\" is already registered.");

        var instance = new U();
        systems.Add(instance);
        AddInternal(instance);
    }

    public void Unregister<U>()
        where U : T, new()
    {
        var instances = systems.OfType<U>();

        if (!instances.Any())
            throw new InvalidOperationException($"Game system \"{typeof(T)}\" is not registered.");

        var instance = instances.Single();
        systems.Remove(instance);
        RemoveInternal(instance);
    }

    protected sealed override void Unload()
    {
        systems.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return systems.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
