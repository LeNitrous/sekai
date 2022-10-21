// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine;

public class SceneController : GameSystem, IUpdateable, IRenderable
{
    private readonly List<Scene> scenes = new();

    /// <summary>
    /// Gets or sets the scenes for this scene controller.
    /// </summary>
    public IEnumerable<Scene> Scenes
    {
        get => scenes;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets the only scene for this scene controller.
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
    /// Adds a scene to this scene controller.
    /// </summary>
    public void Add(Scene scene)
    {
        if (scenes.Contains(scene))
            throw new InvalidOperationException(@"This scene is already added to this manager.");

        scenes.Add(scene);
    }

    /// <summary>
    /// Removes a scene from this scene controller.
    /// </summary>
    public bool Remove(Scene scene)
    {
        return scenes.Remove(scene);
    }

    /// <summary>
    /// Adds a range of scenes to this scene controller.
    /// </summary>
    public void AddRange(IEnumerable<Scene> scenes)
    {
        foreach (var scene in scenes)
            Add(scene);
    }

    /// <summary>
    /// Removes a range of scenes from this scene controller.
    /// </summary>
    public void RemoveRange(IEnumerable<Scene> scenes)
    {
        foreach (var scene in scenes)
            Remove(scene);
    }

    /// <summary>
    /// Removes all scenes from this scene controller.
    /// </summary>
    public void Clear()
    {
        RemoveRange(scenes);
    }

    public void Render()
    {
        foreach (var scene in scenes)
            scene.Render();
    }

    public void Update()
    {
        foreach (var scene in scenes)
            scene.Update();
    }
}
