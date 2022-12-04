// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

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

            enabled = value;

            if (IsAttached)
            {
                invokeState();
            }
        }
    }

    private bool enabled = true;

    /// <summary>
    /// Toggles the active state of this object.
    /// </summary>
    public void Toggle() => Enabled = !Enabled;

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

    private void invokeState()
    {
        if (enabled)
        {
            OnActivate();
        }
        else
        {
            OnDeactivate();
        }
    }

    protected override void OnAttach()
    {
        invokeState();
    }

    protected override void Destroy()
    {
        Enabled = false;
        base.Destroy();
    }
}
