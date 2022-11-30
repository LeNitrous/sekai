// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections;
using System.Collections.Generic;

namespace Sekai.Scenes;

/// <summary>
/// A collection of scenes.
/// </summary>
public class SceneCollection : FrameworkObject, ICollection<Scene>, IReadOnlyList<Scene>
{
    public Scene this[int index] => scenes[index];
    public int Count => scenes.Count;
    public bool IsReadOnly => false;

    private readonly List<Scene> scenes = new();

    public void Add(Scene item)
    {
        if (item.IsAttached)
            return;

        scenes.Add(item);
        item.Attach(this);
    }

    public void Clear()
    {
        foreach (var scene in scenes)
            scene.Detach(this);

        scenes.Clear();
    }

    public bool Contains(Scene item)
    {
        return scenes.Contains(item);
    }

    public void CopyTo(Scene[] array, int arrayIndex)
    {
        scenes.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Scene> GetEnumerator()
    {
        return scenes.GetEnumerator();
    }

    public bool Remove(Scene item)
    {
        if (!item.IsAttached)
            return false;

        if (!scenes.Remove(item))
            return false;

        item.Detach(this);

        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
