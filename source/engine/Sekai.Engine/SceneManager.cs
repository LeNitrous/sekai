// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine;

public class SceneManager : GameSystem
{
    private readonly List<Scene> scenes = new();

    /// <summary>
    /// Gets or sets the scenes for this scene manager.
    /// </summary>
    public IReadOnlyList<Scene> Scenes
    {
        get => scenes;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets the only scene for this scene manager.
    /// </summary>
    public Scene Scene
    {
        get => scenes.Single();
        set
        {
            Clear();
            Add(value);
        }
    }

    /// <summary>
    /// Adds a scene to this scene manager.
    /// </summary>
    public void Add(Scene scene)
    {
        if (scenes.Contains(scene))
            throw new InvalidOperationException(@"This scene is already added to this manager.");

        scenes.Add(scene);
        AddInternal(scene);
    }

    /// <summary>
    /// Removes a scene from this scene manager.
    /// </summary>
    public void Remove(Scene scene)
    {
        if (!scenes.Contains(scene))
            return;

        scenes.Remove(scene);
        RemoveInternal(scene);
    }

    /// <summary>
    /// Adds a range of scenes to this scene manager.
    /// </summary>
    public void AddRange(IEnumerable<Scene> scenes)
    {
        foreach (var scene in scenes)
            Add(scene);
    }

    /// <summary>
    /// Removes a range of scenes from this scene manager.
    /// </summary>
    public void RemoveRange(IEnumerable<Scene> scenes)
    {
        foreach (var scene in scenes)
            Remove(scene);
    }

    /// <summary>
    /// Removes all scenes from this scene manager.
    /// </summary>
    public void Clear()
    {
        RemoveRange(scenes);
    }
}
