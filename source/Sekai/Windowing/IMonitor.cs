// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Mathematics;

namespace Sekai.Windowing;

public interface IMonitor
{
    /// <summary>
    /// The name of this monitor.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The index of this monitor.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// The bounds of this monitor.
    /// </summary>
    Rectangle Bounds { get; }

    /// <summary>
    /// The available video modes for this monitor.
    /// </summary>
    IReadOnlyList<VideoMode> Modes { get; }
}
