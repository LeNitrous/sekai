// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework;

namespace Sekai.Engine;

public class SystemCollection<T> : FrameworkObject, IEnumerable<T>
    where T : FrameworkObject, IGameSystem
{
    private readonly Dictionary<Type, object> systems = new();

    public object Get(Type type)
    {
        if (!type.IsAssignableTo(typeof(T)))
            throw new InvalidCastException();

        if (!systems.ContainsKey(type))
            throw new KeyNotFoundException($"{type} is not registered in this collection.");

        return systems[type];
    }

    public U Get<U>()
        where U : T
    {
        return (U)Get(typeof(U));
    }

    public object Register(Type type)
    {
        if (!type.IsAssignableTo(typeof(T)))
            throw new InvalidOperationException();

        if (systems.ContainsKey(typeof(T)))
            throw new InvalidOperationException();

        object? instance = Activator.CreateInstance(type);
        systems.Add(type, instance!);

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

        return systems.Remove(type);
    }

    public bool Unregister<U>()
        where U : T
    {
        return Unregister(typeof(U));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return systems.Values.OfType<T>().Where(s => s.Enabled && !s.IsDisposed).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
