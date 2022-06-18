// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

/// <summary>
/// A component that runs once loaded by its parent <see cref="Entities.Entity"/>.
/// </summary>
public abstract class Component : ActivatableObject
{
    /// <summary>
    /// The current scene.
    /// </summary>
    [Resolved]
    public Scene Scene { get; set; } = null!;

    /// <summary>
    /// The owning entity.
    /// </summary>
    public Entity Entity { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the priority of this component.
    /// </summary>
    /// <remarks>
    /// This affects the order in which when the component is processed.
    /// </remarks>
    public int Priority { get; set; }

    public sealed override IServiceContainer Services => Entity.Services;

    /// <inheritdoc cref="OnLoad"/>
    public virtual void Load()
    {
    }

    /// <inheritdoc cref="OnActivate"/>
    public virtual void Activate()
    {
    }

    /// <inheritdoc cref="OnDeactivate"/>
    public virtual void Deactivate()
    {
    }

    /// <inheritdoc cref="OnUnload"/>
    public virtual void Unload()
    {
    }

    protected sealed override void OnLoad()
    {
        Load();
        base.OnLoad();
    }

    protected sealed override void OnUnload()
    {
        Unload();
        base.OnUnload();
    }

    protected sealed override void OnActivate()
    {
        Activate();
    }

    protected sealed override void OnDeactivate()
    {
        Deactivate();
    }
}
