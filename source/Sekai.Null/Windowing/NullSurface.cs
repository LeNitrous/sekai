// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.Null.Windowing;

internal class NullSurface : Surface
{
    public override bool Active => active;
    public override Size2 Size => new(1, 1);
    public override Point Position => Point.Zero;

    public override event Action<bool>? OnStateChanged;
    public override event Action? OnClose;
    public override event Action? OnUpdate;
    public override event Func<bool>? OnCloseRequested;

    private bool active = true;
    private volatile bool alive;

    public void Restore()
    {
        active = true;
        OnStateChanged?.Invoke(active);
    }

    public void Suspend()
    {
        active = false;
        OnStateChanged?.Invoke(active);
    }

    public override void Close()
    {
        if (OnCloseRequested?.Invoke() ?? true)
            alive = false;
    }

    public override Point PointToClient(Point point)
    {
        return Point.Zero;
    }

    public override Point PointToScreen(Point point)
    {
        return Point.Zero;
    }

    public override void Run()
    {
        alive = true;

        while (alive)
        {
            OnUpdate?.Invoke();
        }

        OnClose?.Invoke();
    }
}
