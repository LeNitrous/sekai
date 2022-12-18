// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai;

/// <summary>
/// An object capable of being attached.
/// </summary>
public abstract class AttachableObject : FrameworkObject
{
    public bool IsAttached { get; private set; }

    /// <summary>
    /// Called when the object has been attached.
    /// </summary>
    protected virtual void OnAttach()
    {
    }

    /// <summary>
    /// Called when the object is being detached.
    /// </summary>
    protected virtual void OnDetach()
    {
    }

    internal void Attach()
    {
        if (IsAttached)
            return;

        OnAttach();

        IsAttached = true;
    }

    internal void Detach()
    {
        if (!IsAttached)
            return;

        OnDetach();

        IsAttached = false;
    }

    protected override void Destroy() => Detach();
}
