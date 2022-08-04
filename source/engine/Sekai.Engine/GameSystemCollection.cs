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
public class GameSystemCollection : LoadableObject, IEnumerable<GameSystem>
{
    private readonly List<GameSystem> systems = new();

    public T Get<T>()
        where T : GameSystem, new()
    {
        var instances = systems.OfType<T>();

        if (!instances.Any())
            throw new KeyNotFoundException($"Game system \"{typeof(T)}\" is not registered.");

        return instances.Single();
    }

    public void Register<T>()
        where T : GameSystem, new()
    {
        if (systems.OfType<T>().Any())
            throw new InvalidOperationException($"Game system \"{typeof(T)}\" is already registered.");

        var instance = Activator.CreateInstance<T>();
        systems.Add(instance);
        AddInternal(instance);
    }

    public void Unregister<T>()
        where T : GameSystem, new()
    {
        var instances = systems.OfType<T>();

        if (!instances.Any())
            throw new InvalidOperationException($"Game system \"{typeof(T)}\" is not registered.");

        var instance = instances.Single();
        systems.Remove(instance);
        RemoveInternal(instance);
    }

    protected override void Unload()
    {
        systems.Clear();
    }

    public IEnumerator<GameSystem> GetEnumerator() => systems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => systems.GetEnumerator();
}
