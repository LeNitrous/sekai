// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;

namespace Sekai.Graphics;

/// <summary>
/// The base class for all graphics-related objects used by Sekai which provides a general implementation of an <see cref="IDisposable"/>.
/// </summary>
public abstract class GraphicsObject : IDisposable
{
    /// <summary>
    /// Gets whether this object has been disposed or not.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// The graphics context.
    /// </summary>
    protected readonly GraphicsContext Context = Services.Current.Resolve<GraphicsContext>();

    private bool isQueuedForDisposal;

    protected abstract void Destroy();

    public void Dispose()
    {
        if (isQueuedForDisposal)
            return;

        isQueuedForDisposal = true;

        Context.EnqueueDisposal(() =>
        {
            if (IsDisposed)
                return;

            Destroy();

            IsDisposed = true;
            isQueuedForDisposal = false;
        });

        GC.SuppressFinalize(this);
    }
}
