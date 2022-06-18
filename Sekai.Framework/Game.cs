// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;

namespace Sekai.Framework;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : LoadableObject, IUpdateable, IRenderable
{
    /// <summary>
    /// The currently running game. May be null if host is currently not running.
    /// </summary>
    public static Game Current { get; internal set; } = null!;

    /// <inheritdoc cref="OnLoad"/>
    public virtual void Load()
    {
    }

    /// <inheritdoc cref="IRenderable.Render"/>
    public virtual void Render(CommandList commands)
    {
    }

    /// <inheritdoc cref="IUpdateable.Update"/>
    public virtual void Update(double elapsed)
    {
    }

    /// <inheritdoc cref="OnUnload"/>
    public virtual void Unload()
    {
    }

    protected sealed override void OnLoad()
    {
        Load();
    }

    protected sealed override void OnUnload()
    {
        Unload();
    }
}
