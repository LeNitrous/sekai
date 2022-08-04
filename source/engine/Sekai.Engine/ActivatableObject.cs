// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Allocation;

namespace Sekai.Engine;

public abstract class ActivatableObject : LoadableObject
{
    private bool activated;

    /// <summary>
    /// Gets or sets whether this object should be activated.
    /// </summary>
    public bool Activated
    {
        get => activated;
        set
        {
            if (!IsLoaded)
                throw new InvalidOperationException(@"Cannot activate unloaded activatable objects.");

            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (activated == value)
                return;

            activated = value;

            if (activated)
            {
                Post(activate);
            }
            else
            {
                Post(deactivate);
            }
        }
    }

    /// <summary>
    /// Invoked when this activatable object is activated.
    /// </summary>
    public event Action OnActivate = null!;

    /// <summary>
    /// Invoked when this activatable object is deactivated.
    /// </summary>
    public event Action OnDeactivate = null!;

    public ActivatableObject()
    {
        OnLoad += handleOnLoad;
        OnUnload += handleOnUnload;
    }

    /// <summary>
    /// Called when the activatable object becomes activated.
    /// </summary>
    protected virtual void Activate()
    {
    }

    /// <summary>
    /// Called when the activatable object becomes deactivated.
    /// </summary>
    protected virtual void Deactivate()
    {
    }

    private void activate()
    {
        Activate();
        OnActivate?.Invoke();
    }

    private void deactivate()
    {
        Deactivate();
        OnDeactivate?.Invoke();
    }

    private void handleOnLoad()
    {
        Activated = true;
    }

    private void handleOnUnload()
    {
        Activated = false;
        OnLoad -= handleOnLoad;
        OnUnload -= handleOnUnload;
    }
}
