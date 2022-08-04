// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework.Windowing;

namespace Sekai.SDL;

public class SDLWindow : IWindow, INativeWindow
{
    public nint Handle { get; private set; }
    public bool Focused { get; private set; }
    public string Title { get; set; } = string.Empty;
    public ReadOnlyMemory<byte> Icon { get; set; }
    public Point Position { get; set; }
    public Size Size { get; set; }
    public Size MinimumSize { get; set; }
    public Size MaximumSize { get; set; }
    public bool Resizable { get; set; }
    public bool Visible { get; set; }
    public event Action OnLoad = null!;
    public event Action OnClose = null!;
    public event Func<bool> OnCloseRequested = null!;
    public event Action<Size> OnResize = null!;
    public event Action<bool> OnFocusChanged = null!;
    public event Action<string[]> OnDataDropped = null!;

    public void DoEvents()
    {
    }
}
