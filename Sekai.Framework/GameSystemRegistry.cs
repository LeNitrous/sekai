// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.
using System;
using System.Collections.Generic;

namespace Sekai.Framework;

/// <summary>
/// This class keeps track of the actively registered game classes.
//  Every game system must be registered here, otherwise, it cannot be used.
/// </summary>
public class GameSystemRegistry : IDisposable
{
    private readonly Dictionary<Type, IGameSystem> systems = new();

    /// <summary>
    /// Gets a registered game system.
    /// </summary>
    public T GetSystem<T>() where T : IGameSystem
    {
        return !systems.TryGetValue(typeof(T), out var gs)
            ? throw new InvalidCastException($"No such system of type {typeof(T).Name} is found.")
            : (T)gs;
    }

    /// <summary>
    /// Registers a game system.
    /// </summary>
    public void Register<T>(T system) where T : IGameSystem
    {
        if (systems.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"System of type {typeof(T).Name} is already registered.");

        systems.Add(typeof(T), system);
    }

    /// <summary>
    /// Unregisters a game system. This also disposes it, so be wary if you really wanna do this.
    /// </summary>
    public void Unregister<T>(T system) where T : IGameSystem
    {
        if (!systems.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"System {system} of type {typeof(T).Name} is not registered.");


        systems.Remove(typeof(T));
    }

    /// <summary>
    /// Returns an enumerator for the registry.
    /// </summary>
    public Dictionary<Type, IGameSystem>.Enumerator GetEnumerator()
    {
        return systems.GetEnumerator();
    }

    /// <summary>
    /// Disposes the Registry and it's registered components.
    /// </summary>
    public void Dispose()
    {
        // we can't leave any game systems dangling
        // so we'll have to dispose them immediately on dispose.
        foreach (var item in systems.Values)
            item.Dispose();

        systems.Clear();
        GC.SuppressFinalize(this);
    }
}
