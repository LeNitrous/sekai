// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

public abstract class Component : ActivatableObject
{
    [Resolved]
    public Entity Entity { get; set; } = null!;

    [Resolved]
    public Scene Scene { get; set; } = null!;

    public virtual void Load()
    {
    }

    public virtual void Activate()
    {
    }

    public virtual void Deactivate()
    {
    }

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

    protected sealed override void OnEnable()
    {
        Activate();
    }

    protected sealed override void OnDisable()
    {
        Deactivate();
    }
}
