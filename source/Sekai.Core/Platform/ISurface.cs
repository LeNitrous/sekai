// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;

namespace Sekai.Platform;

/// <summary>
/// An interface for surface objects.
/// </summary>
public interface ISurface
{
    /// <summary>
    /// The surface's size.
    /// </summary>
    Size Size { get; }

    /// <summary>
    /// Translates screen-space coordinates to local coordinates.
    /// </summary>
    /// <param name="point">The local-space coordinates.</param>
    /// <returns>The screen-space coordinates.</returns>
    Point PointToClient(Point point);

    /// <summary>
    /// Translates local-space coordinates to screen coordinates.
    /// </summary>
    /// <param name="point">The local-space coordinates.</param>
    /// <returns>The screen-space coordinates.</returns>
    Point PointToScreen(Point point);
}
