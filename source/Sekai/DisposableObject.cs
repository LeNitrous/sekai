// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// Base class that implements the dispose pattern for all objects that have resources that can be released.
/// </summary>
public abstract class DisposableObject : IDisposable
{
    /// <summary>
    /// Gets whether this object has been disposed or not.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// <param name="disposing">True if managed resources should be disposed. Otherwise false.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    private void destroy(bool disposing)
    {
        if (IsDisposed)
            return;

        Dispose(disposing);

        IsDisposed = true;
    }

    ~DisposableObject()
    {
        destroy(false);
    }

    public void Dispose()
    {
        destroy(true);
        GC.SuppressFinalize(this);
    }
}
