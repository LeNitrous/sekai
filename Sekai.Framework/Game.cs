// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Framework;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : LoadableObject, IUpdateable, IRenderable
{
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
