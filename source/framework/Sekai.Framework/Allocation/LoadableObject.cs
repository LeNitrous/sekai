// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Allocation;

/// <summary>
/// The base class of all objects interacting with the dependency injection system.
/// Loadable object fields and properties may have either the <see cref="Annotations.ResolvedAttribute"/>
/// to resolve dependencies from parent loadable objects or <see cref="Annotations.CachedAttribute"/> to
/// cache dependencies for their children to use.
/// </summary>
public abstract partial class LoadableObject : FrameworkObject
{
    /// <summary>
    /// Whether this given loadable object is loaded or not.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Loads this given loadable object after dependencies have been injected and its children have loaded.
    /// </summary>
    protected virtual void Load()
    {
    }

    /// <summary>
    /// Unloads this given loadable object and is called during disposal.
    /// </summary>
    protected virtual void Unload()
    {
    }
}
