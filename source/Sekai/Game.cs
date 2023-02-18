// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Scenes;

namespace Sekai;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : ServiceableObject
{
    /// <summary>
    /// Prepares a game for running.
    /// </summary>
    public static GameBuilder Setup<T>(GameOptions options)
        where T : Game, new()
    {
        return new GameBuilder(() => new T(), options);
    }

    /// <summary>
    /// The scene collection.
    /// </summary>
    public SceneCollection Scenes { get; } = new();

    /// <summary>
    /// Called as the game starts.
    /// </summary>
    public virtual void Load()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <remarks>
    /// By default, it renders the <see cref="SceneCollection"/>. Override this behavior if unwanted.
    /// </remarks>
    public virtual void Render()
    {
        Scenes.Render();
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <remarks>
    /// By default, it updates the <see cref="SceneCollection"/>. Override this behavior if unwanted.
    /// </remarks>
    public virtual void Update()
    {
        Scenes.Update();
    }

    /// <summary>
    /// Called before the game exits.
    /// </summary>
    public virtual void Unload()
    {
    }

    protected sealed override void Dispose(bool disposing)
    {
        if (disposing)
            Unload();

        base.Dispose(disposing);
    }
}
