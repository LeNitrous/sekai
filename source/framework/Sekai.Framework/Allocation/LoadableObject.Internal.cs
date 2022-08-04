// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Containers;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Allocation;

public partial class LoadableObject
{
    private protected virtual bool CanCache => true;
    private protected virtual bool CanResolve => true;
    protected internal IReadOnlyContainer Container { get; private set; } = null!;
    internal LoadableObject Parent = null!;
    private FrameworkThread thread = null!;
    private List<LoadableObject> loadables = null!;
    private readonly object syncLock = new();
    private bool pendingLoad;
    private bool pendingUnload;

    private protected IReadOnlyList<LoadableObject> Loadables
    {
        get
        {
            if (loadables == null)
                return Array.Empty<LoadableObject>();

            return loadables;
        }
    }

    /// <summary>
    /// Invokes a given action in a thread.
    /// </summary>
    private protected void Post(Action action)
    {
        if (thread != null)
        {
            thread.Post(action);
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// Initializes the given loadable for use.
    /// </summary>
    internal void Initialize(LoadableObject parent = null!, FrameworkThread thread = null!)
    {
        if (IsLoaded || pendingLoad)
            return;

        this.thread = thread;

        pendingLoad = true;

        Post(() => load(parent));
    }

    /// <summary>
    /// Adds a child loadable for this loadable.
    /// </summary>
    internal void AddInternal(LoadableObject child)
    {
        if (loadables == null)
            loadables = new();

        if (child == null)
            throw new NullReferenceException(nameof(child));

        if (Parent == child || loadables.Contains(child))
            throw new InvalidOperationException($"This {nameof(child)} cannot be added to this loadable.");

        lock (syncLock)
        {
            loadables.Add(child);
        }

        if (IsLoaded)
            child.Initialize(this, thread);
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

        if (Parent == child || !loadables.Contains(child))
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
        if (pendingUnload)
            return;

        pendingUnload = true;

        Post(unload);
    }

    private void load(LoadableObject parent = null!)
    {
        if (parent != null)
        {
            if (parent.IsDisposed)
                throw new ObjectDisposedException(nameof(parent));

            if (!parent.IsLoaded)
                throw new InvalidOperationException(@"The owning loadable is not loaded.");

            Parent = parent;
            Container = new Container(parent.Container);
        }
        else
        {
            Container = new Container();
        }

        var type = GetType();

        if (!metadata.TryGetValue(type, out var data))
        {
            data = new LoadableMetadata(type);
            metadata.Add(type, data);
        }

        var container = Container as IContainer;

        if (container != null && CanResolve)
            data.Resolve(this, container);

        IsLoaded = true;

        Load();
        OnLoad?.Invoke();

        if (container != null && CanCache)
            data.Cache(this, container);

        if (loadables != null)
        {
            lock (syncLock)
            {
                foreach (var loadable in loadables)
                    loadable.Initialize(this, thread);
            }
        }

        pendingLoad = false;
    }

    private void unload()
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
        pendingUnload = false;
    }
}
