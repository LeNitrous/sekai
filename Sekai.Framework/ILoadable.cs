// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Framework;

internal interface ILoadable
{
    LoadableObject? Parent { get; }
    IReadOnlyList<LoadableObject> Children { get; }
    void Load();
    void Add(LoadableObject loadable);
    void AddRange(IEnumerable<LoadableObject> loadables);
    void Remove(LoadableObject loadable);
    void RemoveRange(IEnumerable<LoadableObject> loadables);
    void Clear();
}
