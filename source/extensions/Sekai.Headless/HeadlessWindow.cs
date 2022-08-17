using System;
using System.Collections.Generic;
using System.Drawing;
using Sekai.Engine.Testing;
using Sekai.Framework.Windowing;

namespace Sekai.Headless;

public class HeadlessWindow : HeadlessView, IWindow
{
    public string Title { get; set; } = @"Headless Window";
    public Icon Icon { get; set; }
    public Point Position { get; set; }
    public Size Size { get; set; }
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

    public HeadlessWindow()
    {
        Monitor = new HeadlessMonitor();
        Monitors = new[] { Monitor };
    }
}
