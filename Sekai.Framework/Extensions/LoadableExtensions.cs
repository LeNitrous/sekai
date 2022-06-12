// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Framework.Extensions;

internal static class LoadableExtensions
{
    public static void Add(this LoadableObject thisLoadable, LoadableObject loadable)
    {
        ((ILoadable)thisLoadable).Add(loadable);
    }

    public static void AddRange(this LoadableObject thisLoadable, IEnumerable<LoadableObject> loadables)
    {
        ((ILoadable)thisLoadable).AddRange(loadables);
    }

    public static void Remove(this LoadableObject thisLoadable, LoadableObject loadable)
    {
        ((ILoadable)thisLoadable).Remove(loadable);
    }

    public static void RemoveRange(this LoadableObject thisLoadable, IEnumerable<LoadableObject> loadables)
    {
        ((ILoadable)thisLoadable).RemoveRange(loadables);
    }

    public static void Clear(this LoadableObject thisLoadable)
    {
        ((ILoadable)thisLoadable).Clear();
    }
}
