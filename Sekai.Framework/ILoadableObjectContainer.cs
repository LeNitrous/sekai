// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Framework;

internal interface ILoadableObjectContainer
{
    LoadableObject? Parent { get; }
    IReadOnlyList<LoadableObject> Children { get; }
    void Add(LoadableObject loadable);
    void Remove(LoadableObject loadable);
}
