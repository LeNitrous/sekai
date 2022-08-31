// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Containers;

namespace Sekai.Framework.Allocation;

public partial class LoadableObject
{
    /// <summary>
    /// Gets whether this loadable object is alive.
    /// </summary>
    internal virtual bool IsAlive => IsLoaded;

    /// <summary>
    /// Invoked when this loadable object is loaded.
    /// </summary>
    internal event Action OnLoad = null!;

    /// <summary>
    /// Invoked when this loadable object is being unloaded.
    /// </summary>
    internal event Action OnUnload = null!;

    /// <summary>
    /// The dependency container.
    /// </summary>
    internal readonly Container Container = new();

    /// <summary>
    /// The parent loadable object that loaded this.
    /// </summary>
    internal LoadableObject Parent = null!;

    private List<LoadableObject> loadables = null!;
    private readonly object syncLock = new();

    private protected IReadOnlyList<LoadableObject> Loadables => loadables ?? (Array.Empty<LoadableObject>() as IReadOnlyList<LoadableObject>);

    /// <summary>
    /// Initializes the given loadable for use.
    /// </summary>

    internal void Initialize()
    {
        Initialize(null!);
    }

    /// <summary>
    /// Initializes the given loadable for use.
    /// </summary>
    internal void Initialize(LoadableObject parent = null!)
    {
        if (IsLoaded)
            return;


        if (parent != null)
        {
            if (parent.IsDisposed)
                throw new ObjectDisposedException(nameof(parent));

            if (!parent.IsLoaded)
                throw new InvalidOperationException(@"The owning loadable is not loaded.");

            Parent = parent;
            Container.Parent = parent.Container;
        }

        var type = GetType();

        if (!metadata.TryGetValue(type, out var data))
        {
            data = new LoadableMetadata(type);
            metadata.Add(type, data);
        }

        data.Load(this);
        IsLoaded = true;

        if (loadables != null)
        {
            lock (syncLock)
            {
                foreach (var loadable in loadables)
                    loadable.Initialize(this);
            }
        }

        Load();
        OnLoad?.Invoke();
    }

    /// <summary>
    /// Adds a child loadable for this loadable.
    /// </summary>
    internal void AddInternal(LoadableObject child)
    {
        if (child == null)
            throw new NullReferenceException(nameof(child));

        if (Parent == child || Loadables.Contains(child))
            throw new InvalidOperationException($"This {nameof(child)} cannot be added to this loadable.");

        lock (syncLock)
        {
            loadables ??= new();
            loadables.Add(child);
        }

        if (IsLoaded)
            child.Initialize(this);
    }

    /// <summary>
    /// Removes and disposes a child loadable from this loadable.
    /// </summary>
    internal void RemoveInternal(LoadableObject child)
    {
        if (loadables == null)
            return;

        if (child == null)
            throw new NullReferenceException(nameof(child));

        if (Parent == child || !Loadables.Contains(child))
            throw new InvalidOperationException($"This {nameof(child)} cannot be removed from this loadable.");

        lock (syncLock)
        {
            loadables.Remove(child);
        }

        if (!IsDisposed)
            child.Dispose();
    }

    protected sealed override void Destroy()
    {
        OnUnload?.Invoke();

        if (loadables != null)
        {
            lock (syncLock)
            {
                foreach (var loadable in loadables)
                    loadable.Dispose();
            }

            loadables.Clear();
        }

        if (Container is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Unload();

        Parent = null!;
        IsLoaded = false;
    }
}
