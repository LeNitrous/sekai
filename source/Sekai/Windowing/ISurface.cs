// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Windowing;

public interface ISurface
{
    /// <summary>
    /// The active state of this view.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Gets or sets the size of the view.
    /// </summary>
    Size2 Size { get; }

    /// <summary>
    /// Gets or sets the position of the window.
    /// </summary>
    Point Position { get; }

    /// <summary>
    /// Convert this point to screen coordinates.
    /// </summary>
    Point PointToScreen(Point point);

    /// <summary>
    /// Convert this point to client coordinates.
    /// </summary>
    Point PointToClient(Point point);
}
