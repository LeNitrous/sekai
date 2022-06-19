// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Systems;

namespace Sekai.Framework.Entities;

/// <summary>
/// A game system that manages loading and unloading <see cref="Scene"/>s.
/// </summary>
public class SceneManager : GameSystem, IUpdateable, IRenderable
{
    private Scene? current;
    private static readonly Logger logger = Logger.GetLogger("Scenes");

    /// <summary>
    /// Gets or sets the current scene.
    /// </summary>
    /// <remarks>
    /// The scene is implicitly loaded if it has not been loaded yet
    /// and implicitly unloads the previous scene.
    /// </remarks>
    public Scene? Current
    {
        get => current;
        set
        {
            if (current == value)
                return;

            if (current != null)
                Unload(current);

            logger.Info(@$"Switching scenes ""{current?.Name ?? "null"}"" → ""{value?.Name ?? "null"}"".");
            current = value;

            if (current != null)
                Load(current);
        }
    }

    /// <summary>
    /// Loads a given scene for use later.
    /// </summary>
    public void Load(Scene scene)
    {
        if (scene == null)
            throw new ArgumentNullException(nameof(scene));

        if (scene.Manager == this)
            return;

        logger.Info(@$"Loading scene ""{scene.Name}"".");

        scene.Manager = this;
        AddInternal(scene);
    }

    /// <summary>
    /// Unloads an owned scene.
    /// </summary>
    public void Unload(Scene scene)
    {
        if (scene == null)
            throw new ArgumentNullException(nameof(scene));

        if (scene.Manager == null)
            return;

        if (scene.Manager != this)
            throw new InvalidOperationException(@"Cannot unload a scene not owned by this scene manager.");

        logger.Info(@$"Unloading scene ""{scene.Name}"".");

        scene.Manager = null;
        RemoveInternal(scene);
    }

    void IUpdateable.Update(double elapsed)
    {
        (Current as IUpdateable)?.Update(elapsed);
    }

    void IRenderable.Render(CommandList commands)
    {
        (Current as IRenderable)?.Render(commands);
    }

    public void Render(CommandList commands)
    {
        throw new NotImplementedException();
    }
}
