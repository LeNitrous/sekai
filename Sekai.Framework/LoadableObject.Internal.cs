// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Collections;

namespace Sekai.Framework;

public partial class LoadableObject
{
    internal IReadOnlyList<LoadableObject> InternalChildren => loadables?.ToArray() ?? Array.Empty<LoadableObject>();
    internal LoadableObject? InternalParent => parent;
    private WeakCollection<LoadableObject>? loadables;
    private static readonly Dictionary<Type, LoadableMetadata> metadatas = new();

    /// <summary>
    /// Loads this loadable and its children.
    /// </summary>
    internal void LoadInternal()
    {
        if (IsLoaded)
            throw new InvalidOperationException(@"This loadable is already loaded.");

        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot load destroyed loadables.");

        var type = GetType();

        if (!metadatas.TryGetValue(type, out var metadata))
        {
            metadata = new LoadableMetadata(type);
            metadatas.Add(type, metadata);
        }

        metadata.Load(this);

        OnLoad();

        if (loadables != null)
        {
            lock (loadables)
            {
                foreach (var loadable in loadables)
                    loadable.LoadInternal();
            }
        }

        IsLoaded = true;
    }

    /// <summary>
    /// Adds a loadable object to be part of its hierarchy.
    /// </summary>
    internal void AddInternal(LoadableObject loadable)
    {
        if (loadable == this)
            throw new InvalidOperationException(@"Cannot add itself as a loadable.");

        loadables ??= new();

        lock (loadables)
        {
            if (loadables.Contains(loadable))
                return;

            loadables.Add(loadable);
        }

        loadable.parent = this;
        loadable.Services.Parent = Services;

        if (IsLoaded)
            loadable.LoadInternal();
    }

    /// <summary>
    /// Adds a range of loadable objects to be part of its hierarchy.
    /// </summary>
    internal void AddRangeInternal(IEnumerable<LoadableObject> loadables)
    {
        foreach (var loadable in loadables)
            AddInternal(loadable);
    }

    /// <summary>
    /// Removes a loadable object from its hierarchy.
    /// </summary>
    internal void RemoveInternal(LoadableObject loadable)
    {
        if (loadables == null)
            return;

        lock (loadables)
        {
            if (!loadables.Contains(loadable))
                return;

            loadables.Remove(loadable);
        }

        loadable.Dispose();
    }

    /// <summary>
    /// Removes a range of loadable objects from its hierarchy.
    /// </summary>
    internal void RemoveRangeInternal(IEnumerable<LoadableObject> loadables)
    {
        foreach (var loadable in loadables)
            RemoveInternal(loadable);
    }

    /// <summary>
    /// Removes all loadable objects from its hierarchy.
    /// </summary>
    internal void ClearInternal()
    {
        RemoveRangeInternal(InternalChildren);
    }
}
