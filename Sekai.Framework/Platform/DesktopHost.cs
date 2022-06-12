// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Framework.Threading;
using Silk.NET.Windowing;

namespace Sekai.Framework.Platform;

public class DesktopHost : ViewHost
{
    public event Action<Vector2>? OnMove;
    public event Action<string[]>? OnFileDrop;
    public event Action<WindowState>? OnStateChange;
    protected new IWindow View => (IWindow)base.View;

    protected override void Initialize(Game game)
    {
        View.IsVisible = false;
        View.Move += e => OnMove?.Invoke((Vector2)e);
        View.FileDrop += e => OnFileDrop?.Invoke(e);
        View.StateChanged += e => OnStateChange?.Invoke(e);

        base.Initialize(game);
    }

    protected override IView CreateView(ViewOptions opts)
    {
        return Window.Create(new WindowOptions(opts));
    }

    protected override GameThread CreateMainThread()
    {
        var thread = base.CreateMainThread();
        thread.Dispatch(() => View.IsVisible = true);
        return thread;
    }
}
