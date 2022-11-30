// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// An object capable of being activated.
/// </summary>
public abstract class ActivateableObject : AttachableObject
{
    public bool Enabled
    {
        get => enabled;
        set
        {
            if (enabled == value)
                return;

            if (enabled = value)
            {
                OnActivate();
            }
            else
            {
                OnDeactivate();
            }
        }
    }

    public event Action<bool>? OnStateChanged;

    private bool enabled;

    /// <summary>
    /// Called when the object is activated.
    /// </summary>
    protected virtual void OnActivate()
    {
    }

    /// <summary>
    /// Called when the object is deactivated.
    /// </summary>
    protected virtual void OnDeactivate()
    {
    }

    protected override void OnAttach()
    {
        Enabled = true;
    }

    protected override void Destroy()
    {
        Enabled = false;
        base.Destroy();
    }
}
