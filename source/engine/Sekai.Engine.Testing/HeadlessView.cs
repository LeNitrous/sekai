// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Testing;

public class HeadlessView : FrameworkObject, IView
{
    public IMonitor Monitor { get; } = new HeadlessMonitor();
    public VideoMode VideoMode => Monitor.Modes[0];
    public event Action OnClose = null!;
    public event Func<bool> OnCloseRequested = null!;
    public event Action<bool> OnFocusChanged = null!;

    private bool focused;

    public bool Focused
    {
        get => focused;
        set
        {
            if (focused == value)
                return;

            focused = value;
            OnFocusChanged?.Invoke(focused);
        }
    }

    public void Close()
    {
        if (OnCloseRequested?.Invoke() ?? false)
            OnClose?.Invoke();
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
