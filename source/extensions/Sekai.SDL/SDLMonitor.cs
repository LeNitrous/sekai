// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Drawing;
using Sekai.Framework.Windowing;

namespace Sekai.SDL;

internal struct SDLMonitor : IMonitor
{
    public string Name { get; }
    public int Index { get; }
    public Rectangle Bounds { get; }
    public IReadOnlyList<VideoMode> Modes { get; }

    public SDLMonitor(int index, string name, Rectangle bounds, IReadOnlyList<VideoMode> modes)
    {
        Name = name;
        Modes = modes;
        Index = index;
        Bounds = bounds;
    }
}
