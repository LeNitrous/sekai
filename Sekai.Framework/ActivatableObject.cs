// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework;

public class ActivatableObject : LoadableObject
{
    public bool IsActive { get; private set; }

    internal void Activate()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot activate destroyed loadables.");

        if (!IsLoaded)
            throw new InvalidOperationException(@"This loadable is not yet loaded.");

        if (IsActive)
            throw new InvalidOperationException(@"This loadable is already activated.");

        IsActive = true;
        OnActivate();
    }

    internal void Deactivate()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot deactivate destroyed loadables.");

        if (!IsLoaded)
            throw new InvalidOperationException(@"This loadable is not yet loaded.");

        if (!IsActive)
            throw new InvalidOperationException(@"This loadable is not activated.");

        IsActive = false;
        OnDeactivate();
    }

    protected virtual void OnActivate()
    {
    }

    protected virtual void OnDeactivate()
    {
    }

    protected override void Destroy()
    {
        if (IsActive)
            Deactivate();

        base.Destroy();
    }
}
