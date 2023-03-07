// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;

namespace Sekai.Surfaces;

/// <summary>
/// An interface for objects that handle arbitrary surfaces.
/// </summary>
public interface ISurface
{
    /// <summary>
    /// The surface's size.
    /// </summary>
    Size Size { get; }

    /// <summary>
    /// Converts client-space coordinates to screen-space coordinates.
    /// </summary>
    /// <param name="point">The client-space coordinates.</param>
    /// <returns>The point as screen-space coordinates.</returns>
    Point PointToScreen(Point point);

    /// <summary>
    /// Converts screen-space coordinates to client-space coordinates.
    /// </summary>
    /// <param name="point">The screen-space coordinates.</param>
    /// <returns>The point as client-space coordinates.</returns>
    Point PointToClient(Point point);
}
