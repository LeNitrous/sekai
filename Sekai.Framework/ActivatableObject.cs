// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework;

public class ActivatableObject : LoadableObject
{
    private bool isEnabled;

    public bool Enabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled == value)
                return;

            isEnabled = value;

            if (isEnabled)
            {
                enable();
            }
            else
            {
                disable();
            }
        }
    }

    private void enable()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot activate destroyed loadables.");

        if (!IsLoaded)
            throw new InvalidOperationException(@"This loadable is not yet loaded.");

        OnEnable();
    }

    private void disable()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot deactivate destroyed loadables.");

        if (!IsLoaded)
            throw new InvalidOperationException(@"This loadable is not yet loaded.");

        OnDisable();
    }

    internal override void Initialize()
    {
        base.Initialize();
        Enabled = true;
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

    protected override void Destroy()
    {
        if (Enabled)
            Enabled = false;

        base.Destroy();
    }
}
