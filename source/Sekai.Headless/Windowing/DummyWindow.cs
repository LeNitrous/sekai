// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.Headless.Windowing;

internal sealed class DummyWindow : IWindow, IHasSuspend, IHasRestart
{
    public bool Exists { get; private set; } = true;
    public NativeWindowInfo Surface { get; } = new NativeWindowInfo();
    public IMonitor Monitor => DummyMonitor.Instance;
    public WindowBorder Border { get; set; }
    public Size MinimumSize { get; set; }
    public Size MaximumSize { get; set; }
    public bool HasFocus { get; private set; }
    public bool Visible { get; set; }
    public string Title { get; set; } = "Dummy";

    public Size Size
    {
        get => Size.Zero;
        set => Resized?.Invoke(value);
    }

    public WindowState State
    {
        get => WindowState.Normal;
        set => StateChanged?.Invoke(value);
    }

    public Point Position
    {
        get => Point.Zero;
        set => Moved?.Invoke(value);
    }

    public event Action? Closed;
    public event Action? Closing;
    public event Func<bool>? CloseRequested;
    public event Action<Size>? Resized;
    public event Action<Point>? Moved;
    public event Action<bool>? FocusChanged;
    public event Action<WindowState>? StateChanged;
    public event Action? Resumed;
    public event Action? Suspend;
    public event Action? Restart;

    public void Close()
    {
        if (CloseRequested?.Invoke() ?? false)
        {
            return;
        }

        Closing?.Invoke();

        Exists = false;

        Closed?.Invoke();
    }

    public void Dispose()
    {
    }

    public void DoEvents()
    {
    }

    public void Focus()
    {
        if (!HasFocus)
        {
            FocusChanged?.Invoke(HasFocus = true);
        }
    }

    public void Unfocus()
    {
        if (HasFocus)
        {
            FocusChanged?.Invoke(HasFocus = false);
        }
    }

    public void Pause()
    {
        Suspend?.Invoke();
    }

    public void Resume()
    {
        Resumed?.Invoke();
    }

    public void Reload()
    {
        Restart?.Invoke();
    }

    public Point PointToClient(Point point)
    {
        return Point.Zero;
    }

    public Point PointToScreen(Point point)
    {
        return Point.Zero;
    }
}
