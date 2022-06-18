// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Collections;
using Sekai.Framework.Extensions;

namespace Sekai.Framework;

public partial class LoadableObject
{
    IReadOnlyList<LoadableObject> ILoadable.Children => loadables?.ToArray() ?? Array.Empty<LoadableObject>();
    LoadableObject? ILoadable.Parent => parent;
    private WeakCollection<LoadableObject>? loadables;
    private static readonly Dictionary<Type, LoadableMetadata> metadatas = new();

    void ILoadable.Load()
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

        IsLoaded = true;
        OnLoad();

        if (loadables != null)
        {
            foreach (var loadable in loadables)
                loadable.Load();
        }
    }

    void ILoadable.Add(LoadableObject loadable)
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
            loadable.Load();
    }

    void ILoadable.AddRange(IEnumerable<LoadableObject> loadables)
    {
        foreach (var loadable in loadables)
            ((ILoadable)this).Add(loadable);
    }

    void ILoadable.Remove(LoadableObject loadable)
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

    void ILoadable.RemoveRange(IEnumerable<LoadableObject> loadables)
    {
        foreach (var loadable in loadables)
            ((ILoadable)this).Remove(loadable);
    }

    void ILoadable.Clear()
    {
        ((ILoadable)this).RemoveRange(((ILoadable)this).Children);
    }
}
