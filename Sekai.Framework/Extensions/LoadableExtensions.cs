// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Framework.Extensions;

internal static class LoadableExtensions
{
    /// <summary>
    /// Loads this loadable object and its children.
    /// </summary>
    public static void Load(this LoadableObject thisLoadable)
    {
        ((ILoadable)thisLoadable).Load();
    }

    /// <summary>
    /// Adds a loadable object to be loaded during its parent's load.
    /// </summary>
    /// <remarks>
    /// The loadable object is immediately loaded if the parent is already loaded.
    /// </remarks>
    public static void Add(this LoadableObject thisLoadable, LoadableObject loadable)
    {
        ((ILoadable)thisLoadable).Add(loadable);
    }

    /// <summary>
    /// Adds a range of loadable objects to be loaded during their parent's load.
    /// </summary>
    /// <remarks>
    /// The loadable objects are immediately loaded if the parent is already loaded.
    /// </remarks>
    public static void AddRange(this LoadableObject thisLoadable, IEnumerable<LoadableObject> loadables)
    {
        ((ILoadable)thisLoadable).AddRange(loadables);
    }

    /// <summary>
    /// Unloads a given loadable object and disposes it.
    /// </summary>
    public static void Remove(this LoadableObject thisLoadable, LoadableObject loadable)
    {
        ((ILoadable)thisLoadable).Remove(loadable);
    }

    /// <summary>
    /// Unloads a range of loadable objects and disposes those.
    /// </summary>
    public static void RemoveRange(this LoadableObject thisLoadable, IEnumerable<LoadableObject> loadables)
    {
        ((ILoadable)thisLoadable).RemoveRange(loadables);
    }

    /// <summary>
    /// Unloads all of of the children and disposes those.
    /// </summary>
    public static void Clear(this LoadableObject thisLoadable)
    {
        ((ILoadable)thisLoadable).Clear();
    }
}
