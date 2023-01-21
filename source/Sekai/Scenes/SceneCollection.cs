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

    private readonly object syncLock = new();
    private readonly List<Scene> scenes = new();

    internal void Update()
    {
        lock (syncLock)
        {
            foreach (var scene in scenes)
                scene.Update();
        }
    }

    internal void Render()
    {
        lock (syncLock)
        {
            foreach (var scene in scenes)
                scene.Render();
        }
    }

    /// <summary>
    /// Adds a scene to this collection.
    /// </summary>
    public void Add(Scene item)
    {
        lock (syncLock)
            scenes.Add(item);
    }

    /// <summary>
    /// Removes all scenes in this collection.
    /// </summary>
    public void Clear()
    {
        lock (syncLock)
            scenes.Clear();
    }

    /// <summary>
    /// Returns whether a given scene is in this collection.
    /// </summary>
    public bool Contains(Scene item)
    {
        lock (syncLock)
            return scenes.Contains(item);
    }

    /// <summary>
    /// Copies a range of scenes to the array at a given index.
    /// </summary>
    public void CopyTo(Scene[] array, int arrayIndex)
    {
        lock (syncLock)
            scenes.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes a scene from this collection.
    /// </summary>
    public bool Remove(Scene item)
    {
        lock (syncLock)
            return scenes.Remove(item);
    }

    public IEnumerator<Scene> GetEnumerator()
    {
        lock (syncLock)
            return scenes.GetEnumerator();
    }

    bool ICollection<Scene>.IsReadOnly => false;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
