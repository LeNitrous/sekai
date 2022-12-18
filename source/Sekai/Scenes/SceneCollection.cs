// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sekai.Graphics;
using Sekai.Rendering;

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

    internal void Update(double delta)
    {
        var scenes = this.scenes.ToArray();

        foreach (var scene in scenes)
        {
            if (scene.Enabled)
                scene.Update(delta);
        }
    }

    internal void Render(GraphicsContext graphics)
    {
        var scenes = this.scenes.OfType<IRenderableScene>().ToArray();

        foreach (var scene in scenes)
        {
            if (scene.Enabled)
                scene.Render(graphics);
        }
    }

    /// <summary>
    /// Adds a scene to this collection.
    /// </summary>
    public void Add(Scene item)
    {
        if (item.IsAttached)
            return;

        scenes.Add(item);
        item.Attach(this);
    }

    /// <summary>
    /// Removes all scenes in this collection.
    /// </summary>
    public void Clear()
    {
        foreach (var scene in scenes)
            scene.Detach(this);

        scenes.Clear();
    }

    /// <summary>
    /// Returns whether a given scene is in this collection.
    /// </summary>
    public bool Contains(Scene item)
    {
        return scenes.Contains(item);
    }

    /// <summary>
    /// Copies a range of scenes to the array at a given index.
    /// </summary>
    public void CopyTo(Scene[] array, int arrayIndex)
    {
        scenes.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Scene> GetEnumerator()
    {
        return scenes.GetEnumerator();
    }

    /// <summary>
    /// Removes a scene from this collection.
    /// </summary>
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
