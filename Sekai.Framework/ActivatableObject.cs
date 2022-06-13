// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework;

public class ActivatableObject : LoadableObject
{
    public bool IsEnabled { get; private set; }

    internal void Enable()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot activate destroyed loadables.");

        if (!IsLoaded)
            throw new InvalidOperationException(@"This loadable is not yet loaded.");

        if (IsEnabled)
            throw new InvalidOperationException(@"This loadable is already activated.");

        IsEnabled = true;
        OnActivate();
    }

    internal void Disable()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot deactivate destroyed loadables.");

        if (!IsLoaded)
            throw new InvalidOperationException(@"This loadable is not yet loaded.");

        if (!IsEnabled)
            throw new InvalidOperationException(@"This loadable is not activated.");

        IsEnabled = false;
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
        if (IsEnabled)
            Disable();

        base.Destroy();
    }
}
