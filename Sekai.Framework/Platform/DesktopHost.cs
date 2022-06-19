// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Framework.Threading;
using Silk.NET.Windowing;

namespace Sekai.Framework.Platform;

/// <summary>
/// A host backed by a <see cref="IWindow"/>.
/// </summary>
public class DesktopHost : ViewHost
{
    public event Action<Vector2>? OnMove;
    public event Action<string[]>? OnFileDrop;
    public event Action<WindowState>? OnStateChange;
    protected new IWindow View => (IWindow)base.View;

    internal DesktopHost(HostOptions? options = null)
        : base(options)
    {
        View.IsVisible = false;
        View.Title = Options.Title;
        View.Move += e => OnMove?.Invoke((Vector2)e);
        View.FileDrop += e => OnFileDrop?.Invoke(e);
        View.StateChanged += e => OnStateChange?.Invoke(e);
    }

    protected override void Initialize(Game game)
    {
        base.Initialize(game);
        var threads = game.Services.Resolve<FrameworkThreadManager>(true);
        threads.Post(() => View.IsVisible = true);
    }

    protected override IView CreateView(ViewOptions opts) => Window.Create(new WindowOptions(opts));
}
