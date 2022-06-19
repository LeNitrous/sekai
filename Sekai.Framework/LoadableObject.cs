// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Services;

namespace Sekai.Framework;

/// <summary>
/// Represents an object that is capable of caching and resolving services with the use of
/// <see cref="CachedAttribute"/> and <see cref="ResolvedAttribute"/> or through <see cref="Services"/>
/// and is also capable of loading children while resolving and caching their own services.
/// </summary>
public abstract partial class LoadableObject : FrameworkObject
{
    private LoadableObject? parent;

    /// <summary>
    /// Whether this loadable object is loaded or not.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Gets the services that are resolvable for this loadable object.
    /// </summary>
    public virtual IServiceContainer Services { get; } = new ServiceContainer();

    /// <summary>
    /// Called after the loadable object has loaded and its services are resolved.
    /// </summary>
    protected virtual void OnLoad()
    {
    }

    /// <summary>
    /// Called after the loadable object is being disposed.
    /// </summary>
    protected virtual void OnUnload()
    {
    }

    protected sealed override void Destroy()
    {
        OnUnload();
        IsLoaded = false;
        parent = null;
        Services.Dispose();
        ClearInternal();
    }
}
