// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Framework.Entities;

public class Entity : FrameworkObject, ILoadable, IUpdateable
{
    public LoadableObject Parent { get; private set; }

    public IReadOnlyList<LoadableObject> Children { get; private set; }

    public void Update(double delta)
    {

    }

    public void Add(LoadableObject lo)
    {

    }

    public void AddRange(IEnumerable<LoadableObject> loc)
    {

    }

    public void Remove(LoadableObject lo)
    {

    }

    public void RemoveRange(IEnumerable<LoadableObject> loc)
    {

    }

    public void Clear()
    {

    }

    public void Destroy()
    {
        Clear();
    }
}
