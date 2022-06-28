// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework;

/// <summary>
/// An object capable of switching states between activated or not.
/// </summary>
public class ActivatableObject : LoadableObject
{
    private bool enabled;

    /// <summary>
    /// Gets or sets whether this activatable object is enabled or not.
    /// </summary>
    public bool Enabled
    {
        get => enabled;
        set
        {
            if (enabled == value)
                return;

            enabled = value;

            if (enabled)
            {
                activate();
            }
            else
            {
                deactivate();
            }
        }
    }

    protected override void OnLoad()
    {
        Enabled = true;
    }

    protected override void OnUnload()
    {
        Enabled = false;
    }

    /// <summary>
    /// Called when this activatable object is enabled.
    /// </summary>
    protected virtual void OnActivate()
    {
    }

    /// <summary>
    /// Called when this activatable object is disabled.
    /// </summary>
    protected virtual void OnDeactivate()
    {
    }

    private void activate()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot activate destroyed loadables.");

        OnActivate();
    }

    private void deactivate()
    {
        if (IsDisposed)
            throw new InvalidOperationException(@"Cannot deactivate destroyed loadables.");

        OnDeactivate();
    }
}
