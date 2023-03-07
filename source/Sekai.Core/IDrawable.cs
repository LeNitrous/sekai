// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai;

/// <summary>
/// An interface for a game system.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Gets whether this <see cref="IDrawable"/> should draw itself.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// The draw order of this <see cref="IDrawable"/> relative to others.
    /// </summary>
    int DrawOrder { get; }

    /// <summary>
    /// Called when <see cref="Visible"/> is changed.
    /// </summary>
    event EventHandler? VisibleChanged;

    /// <summary>
    /// Called when <see cref="DrawOrder"/> is changed.
    /// </summary>
    event EventHandler? DrawOrderChanged;

    /// <summary>
    /// Called when this <see cref="IDrawable"/> draws itself.
    /// </summary>
    /// <param name="elapsed">The time since the last tick.</param>
    void Draw(TimeSpan elapsed);
}
