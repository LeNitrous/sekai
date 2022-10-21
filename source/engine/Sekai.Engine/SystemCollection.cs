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
    private readonly List<IUpdateable> updateables = new();
    private readonly List<IRenderable> renderables = new();
    private readonly Dictionary<Type, T> registry = new();

    public int Count => systems.Count;
    public T this[int index] => systems[index];

    internal SystemCollection()
    {
    }

    public T Get(Type type)
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

        if (instance is IUpdateable updateable)
            updateables.Add(updateable);

        if (instance is IRenderable renderable)
            renderables.Add(renderable);

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

        if (registry.Remove(type, out var system))
        {
            systems.Remove(system);

            if (system is IUpdateable updateable)
                updateables.Remove(updateable);

            if (system is IRenderable renderable)
                renderables.Remove(renderable);
        }

        return false;
    }

    public bool Unregister<U>()
        where U : T
    {
        return Unregister(typeof(U));
    }

    internal void Update()
    {
        for (int i = 0; i < updateables.Count; i++)
            updateables[i].Update();
    }

    internal void Render()
    {
        for (int i = 0; i < renderables.Count; i++)
            renderables[i].Render();
    }

    protected override void Destroy()
    {
        for (int i = 0; i < systems.Count; i++)
            systems[i].Dispose();

        systems.Clear();
        registry.Clear();
        updateables.Clear();
        renderables.Clear();
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
