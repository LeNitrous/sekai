// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// The base class for all objects used by Sekai.
/// </summary>
public abstract class FrameworkObject : IDisposable
{
    /// <summary>
    /// Gets whether this object has been disposed or not.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Called during this object's disposal.
    /// </summary>
    protected virtual void Destroy()
    {
    }

    /// <summary>
    /// Disposes resources used by this object.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
            return;

        Destroy();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}
