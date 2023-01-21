// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Windowing;

public abstract class Surface : DependencyObject, ISurface
{
    public abstract bool Active { get; }
    public abstract Size2 Size { get; }
    public abstract Point Position { get; }
    public abstract event Action<bool>? OnStateChanged;
    public abstract event Action? OnClose;
    public abstract event Action? OnUpdate;
    public abstract event Func<bool>? OnCloseRequested;
    public abstract Point PointToScreen(Point point);
    public abstract Point PointToClient(Point point);
    public abstract void Run();
    public abstract void Close();
}
