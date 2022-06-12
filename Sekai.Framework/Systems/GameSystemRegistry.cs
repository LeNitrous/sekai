// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sekai.Framework.Systems;

/// <summary>
/// This class keeps track of the actively registered game classes.
//  Every game system must be registered here, otherwise, it cannot be used.
/// </summary>
public class GameSystemRegistry : FrameworkObject, IEnumerable<GameSystem>
{
    private readonly Dictionary<Type, GameSystem> systems = new();
    private readonly Game game;

    public GameSystemRegistry(Game game)
    {
        this.game = game;
    }

    /// <summary>
    /// Gets a registered game system.
    /// </summary>
    public T GetSystem<T>()
        where T : GameSystem, new()
    {
        return !systems.TryGetValue(typeof(T), out var gs)
            ? throw new InvalidCastException($"No such system of type {typeof(T).Name} is found.")
            : (T)gs;
    }

    /// <summary>
    /// Registers a game system.
    /// </summary>
    public void Register<T>()
        where T : GameSystem, new()
    {
        if (systems.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"System of type {typeof(T).Name} is already registered.");

        var system = Activator.CreateInstance<T>();
        ((ILoadableObjectContainer)game).Add(system);
        systems.Add(typeof(T), system);
    }

    /// <summary>
    /// Unregisters a game system.
    /// </summary>
    public void Unregister<T>()
        where T : GameSystem, new()
    {
        if (!systems.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"System of type {typeof(T).Name} is not registered.");

        var system = systems[typeof(T)];

        ((ILoadableObjectContainer)game).Remove(system);
        systems.Remove(typeof(T));
        system.Dispose();
    }

    protected override void Destroy()
    {
        // we can't leave any game systems dangling
        // so we'll have to dispose them immediately on dispose.
        foreach (var system in systems.Values)
        {
            ((ILoadableObjectContainer)game).Remove(system);
            system.Dispose();
        }

        systems.Clear();
    }

    public IEnumerator<GameSystem> GetEnumerator()
    {
        return systems.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
