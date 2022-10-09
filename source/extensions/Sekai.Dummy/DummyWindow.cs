// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Drawing;
using Sekai.Framework.Windowing;

namespace Sekai.Dummy;

internal class DummyWindow : DummyView, IWindow
{
    public string Title { get; set; } = @"Dummy Window";
    public Icon Icon { get; set; }
    public Point Position { get; set; }
    public new Size Size { get; set; }
    public Size MinimumSize { get; set; }
    public Size MaximumSize { get; set; }
    public WindowState State { get; set; }
    public WindowBorder Border { get; set; }
    public IMonitor Monitor { get; }
    public IEnumerable<IMonitor> Monitors { get; }
    public bool Visible { get; set; }
    public event Action<Size> OnResize = null!;
    public event Action<Point> OnMoved = null!;
    public event Action<string[]> OnDataDropped = null!;

    public DummyWindow()
    {
        Monitor = new DummyMonitor();
        Monitors = new[] { Monitor };
    }
}
