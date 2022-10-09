// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Drawing;
using Sekai.Framework.Windowing;

namespace Sekai.Dummy;

internal class DummyMonitor : IMonitor
{
    public string Name { get; } = @"Dummy";
    public bool Primary { get; } = true;
    public int Index { get; } = 0;
    public Rectangle Bounds { get; } = Rectangle.Empty;
    public IReadOnlyList<VideoMode> Modes { get; } = new[] { new VideoMode(Size.Empty, 0, 0) };
}
