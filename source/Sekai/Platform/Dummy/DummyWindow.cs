// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Platform.Dummy;

internal sealed class DummyWindow : IWindow
{
    public bool Exists { get; private set; }
    public ISurface? Surface => null;
    public IWindowHost? Parent { get; }
    public IMonitor? Monitor => null;
    public WindowBorder Border { get; set; }
    public WindowState State { get; set; }
    public Size Size { get; set; }
    public Size MinimumSize { get; set; }
    public Size MaximumSize { get; set; }
    public Point Position { get; set; }
    public bool Focus => false;
    public bool Visible { get; set; }
    public string Title { get; set; } = string.Empty;

    public event Action? Closed;
    public event Action? Closing;
    public event Func<bool>? CloseRequested;

#pragma warning disable CS0067

    public event Action<Size>? Resized;
    public event Action<Point>? Moved;
    public event Action<bool>? FocusChanged;
    public event Action? Resume;
    public event Action? Suspend;

#pragma warning restore CS0067

    public DummyWindow()
    {
        Exists = true;
    }

    private DummyWindow(DummyWindow parent)
    {
        Exists = true;
        Parent = parent;
    }

    public void Close()
    {
        if (!Exists)
        {
            return;
        }

        if (CloseRequested?.Invoke() ?? false)
        {
            return;
        }

        Closing?.Invoke();

        Exists = false;

        Closed?.Invoke();
    }

    public IWindow CreateWindow()
    {
        return new DummyWindow(this);
    }

    public void Dispose()
    {
    }

    public void DoEvents()
    {
    }

    public Point PointToClient(Point point)
    {
        return Point.Empty;
    }

    public Point PointToScreen(Point point)
    {
        return Point.Empty;
    }
}
