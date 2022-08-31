// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Annotations;

namespace Sekai.Engine;

public abstract class Component : ActivatableObject
{
    /// <summary>
    /// The owning entity.
    /// </summary>
    [Resolved]
    public Entity Entity { get; internal set; } = null!;

    /// <summary>
    /// The current scene.
    /// </summary>
    [Resolved]
    public Scene Scene { get; internal set; } = null!;

    protected sealed override void Load()
    {
        Scene.OnEntityUpdate(Entity);
        OnComponentLoad();
    }

    protected sealed override void Unload()
    {
        Scene.OnEntityUpdate(Entity);
        OnComponentUnload();
    }

    protected sealed override void Activate()
    {
        OnComponentActivate();
    }

    protected sealed override void Deactivate()
    {
        OnComponentDeactivate();
    }

    private protected virtual void OnComponentLoad()
    {
    }

    private protected virtual void OnComponentUnload()
    {
    }

    private protected virtual void OnComponentActivate()
    {
    }

    private protected virtual void OnComponentDeactivate()
    {
    }
}
