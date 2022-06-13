// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

public abstract class Component : ActivatableObject
{
    [Resolved]
    public Entity Entity { get; private set; } = null!;

    [Resolved]
    public Scene Scene { get; private set; } = null!;

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

    protected sealed override void OnActivate()
    {
        Activate();
    }

    protected sealed override void OnDeactivate()
    {
        Deactivate();
    }
}
