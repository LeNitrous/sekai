// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using Sekai.Framework;

namespace Sekai.Engine;

public class SystemCollection<T> : FrameworkObject, IEnumerable<T>, IReadOnlyList<T>
    where T : FrameworkObject, IGameSystem
{
    private readonly List<T> systems = new();
    private readonly Dictionary<Type, T> registry = new();

    public int Count => systems.Count;
    public T this[int index] => systems[index];

    public IGameSystem Get(Type type)
    {
        if (!type.IsAssignableTo(typeof(T)))
            throw new InvalidCastException();

        if (!registry.ContainsKey(type))
            throw new KeyNotFoundException($"{type} is not registered in this collection.");

        return registry[type];
    }

    public U Get<U>()
        where U : T
    {
        return (U)Get(typeof(U));
    }

    public T Register(Type type)
    {
        if (!type.IsAssignableTo(typeof(T)))
            throw new InvalidOperationException();

        if (registry.ContainsKey(typeof(T)))
            throw new InvalidOperationException();

        var instance = Activator.CreateInstance(type) as T;
        systems.Add(instance!);
        registry.Add(type, instance!);

        return instance!;
    }

    public U Register<U>()
        where U : T
    {
        return (U)Register(typeof(U));
    }

    public bool Unregister(Type type)
    {
        if (!type.IsAssignableTo(typeof(T)))
            throw new InvalidOperationException();

        if (!registry.Remove(type, out var system))
            return false;

        return systems.Remove(system);
    }

    public bool Unregister<U>()
        where U : T
    {
        return Unregister(typeof(U));
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
