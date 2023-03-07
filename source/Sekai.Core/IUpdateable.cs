// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// An interface for a game system.
/// </summary>
public interface IUpdateable
{
    /// <summary>
    /// Gets whether this <see cref="IUpdateable"/> should update itself.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// The update order of this <see cref="IUpdateable"/> relative to others.
    /// </summary>
    int UpdateOrder { get; }

    /// <summary>
    /// Called when <see cref="Enabled"/> is changed.
    /// </summary>
    event EventHandler? EnableChanged;

    /// <summary>
    /// Called when <see cref="UpdateOrder"/> is changed.
    /// </summary>
    event EventHandler? UpdateOrderChanged;

    /// <summary>
    /// Called when this <see cref="IUpdateable"/> updates itself.
    /// </summary>
    /// <param name="elapsed">The time since the last tick.</param>
    void Update(TimeSpan elapsed);
}
