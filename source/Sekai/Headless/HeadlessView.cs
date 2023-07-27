// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.Headless;

internal sealed class HeadlessView : IView
{
    public Size Size
    {
        get => size;
        set
        {
            if (size.Equals(value))
            {
                return;
            }

            size = value;
            Resized?.Invoke(size);
        }
    }

    public event Action? Closed;
    public event Action? Closing;
    public event Func<bool>? CloseRequested;
    public event Action<Size>? Resized;
    public event Action? Resumed;
    public event Action? Suspend;
    public event Action? Tick;

    private Size size;
    private bool exists;
    private bool paused;

    public void Run()
    {
        exists = true;

        while (exists)
        {
            if (paused)
            {
                continue;
            }

            Tick?.Invoke();
        }
    }

    public void Close()
    {
        if (CloseRequested?.Invoke() ?? false)
        {
            return;
        }

        Closing?.Invoke();

        exists = false;

        Closed?.Invoke();
    }

    public void Pause()
    {
        if (paused)
        {
            return;
        }

        paused = true;

        Suspend?.Invoke();
    }

    public void Resume()
    {
        if (!paused)
        {
            return;
        }

        paused = false;

        Resumed?.Invoke();
    }

    public void Dispose()
    {
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
