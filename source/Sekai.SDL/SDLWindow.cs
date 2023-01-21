// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sekai.Mathematics;
using Sekai.Windowing;
using Silk.NET.Maths;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal unsafe class SDLWindow : SDLSurface, IWindow
{
    public bool Focused { get; private set; }
    public IMonitor Monitor => getMonitorFromSDL(Sdl.GetWindowDisplayIndex(Window));
    public IEnumerable<IMonitor> Monitors { get; private set; }
    public event Action<Size2>? OnResize;
    public event Action<Mathematics.Point>? OnMoved;
    public event Action<string[]>? OnDataDropped;
    public event Action<bool>? OnFocusChanged;

    private string title = string.Empty;

    public string Title
    {
        get => title;
        set
        {
            if (title == value)
                return;

            title = value;
            Invoke(() => Sdl.SetWindowTitle(Window, title));
        }
    }

    private bool visible;

    public bool Visible
    {
        get => visible;
        set
        {
            if (visible == value)
                return;

            visible = value;

            if (visible)
            {
                Invoke(() => Sdl.ShowWindow(Window));
            }
            else
            {
                Invoke(() => Sdl.HideWindow(Window));
            }
        }
    }

    private Windowing.Icon icon;

    public Windowing.Icon Icon
    {
        get => icon;
        set
        {
            if (icon == value)
                return;

            icon = value;
            Invoke(updateWindowIcon);
        }
    }

    public new Mathematics.Point Position
    {
        get => base.Position;
        set => Invoke(() => Sdl.SetWindowPosition(Window, value.X, value.Y));
    }

    public new Size2 Size
    {
        get => base.Size;
        set => Invoke(() => Sdl.SetWindowSize(Window, value.Width, value.Height));
    }

    public Size2 MaximumSize
    {
        get
        {
            int w = 0;
            int h = 0;
            Sdl.GetWindowMaximumSize(Window, ref w, ref h);
            return new Size2(w, h);
        }
        set => Invoke(() => Sdl.SetWindowMaximumSize(Window, value.Width, value.Height));
    }

    public Size2 MinimumSize
    {
        get
        {
            int w = 0;
            int h = 0;
            Sdl.GetWindowMinimumSize(Window, ref w, ref h);
            return new Size2(w, h);
        }
        set => Invoke(() => Sdl.SetWindowMinimumSize(Window, value.Width, value.Height));
    }

    public WindowState State
    {
        get => ((WindowFlags)Sdl.GetWindowFlags(Window)).ToWindowState();
        set => Invoke(() => updateWindowState(value));
    }

    public WindowBorder Border
    {
        get => ((WindowFlags)Sdl.GetWindowFlags(Window)).ToWindowBorder();
        set => Invoke(() => updateWindowBorder(value));
    }

    public SDLWindow()
    {
        updateMonitors();
    }

    protected override void ProcessEvent(Event evt)
    {
        base.ProcessEvent(evt);

        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Windowevent:
                handleWindowEvent(evt.Window);
                break;

            case EventType.Displayevent:
                updateMonitors();
                break;

            case EventType.Dropfile:
            case EventType.Droptext:
            case EventType.Dropbegin:
            case EventType.Dropcomplete:
                handleDropEvent(evt.Drop);
                break;
        }
    }

    [MemberNotNull(nameof(Monitors))]
    private void updateMonitors() => Monitors = Enumerable.Range(0, Sdl.GetNumVideoDisplays()).Select(getMonitorFromSDL);

    private void updateWindowState(WindowState state)
    {
        switch (state)
        {
            case WindowState.Normal:
                Sdl.RestoreWindow(Window);
                break;

            case WindowState.Minimized:
                Sdl.MinimizeWindow(Window);
                break;

            case WindowState.Maximized:
                Sdl.MaximizeWindow(Window);
                break;

            case WindowState.Fullscreen:
                {
                    if (Sdl.SetWindowFullscreen(Window, (uint)WindowFlags.Fullscreen) > 0)
                        Sdl.ThrowError();

                    break;
                }
        }
    }

    private void updateWindowBorder(WindowBorder border)
    {
        switch (border)
        {
            case WindowBorder.Resizable:
                {
                    Sdl.SetWindowBordered(Window, SdlBool.True);
                    Sdl.SetWindowResizable(Window, SdlBool.True);
                    break;
                }

            case WindowBorder.Fixed:
                {
                    Sdl.SetWindowBordered(Window, SdlBool.True);
                    Sdl.SetWindowResizable(Window, SdlBool.False);
                    break;
                }

            case WindowBorder.Hidden:
                {
                    Sdl.SetWindowBordered(Window, SdlBool.False);
                    Sdl.SetWindowResizable(Window, SdlBool.False);
                    break;
                }
        }
    }

    private unsafe void updateWindowIcon()
    {
        Silk.NET.SDL.Surface* surface;

        fixed (void* ptr = icon.Data.Span)
            surface = Sdl.CreateRGBSurfaceFrom(ptr, icon.Width, icon.Height, 32, icon.Width * 4, 0xff000000, 0x00ff0000, 0x0000ff00, 0x000000ff);

        Sdl.SetWindowIcon(Window, surface);
        Sdl.FreeSurface(surface);
    }

    private void handleWindowEvent(WindowEvent evt)
    {
        var type = (WindowEventID)evt.Event;

        switch (type)
        {
            case WindowEventID.Moved:
                OnMoved?.Invoke(Position);
                break;

            case WindowEventID.SizeChanged:
                OnResize?.Invoke(Size);
                break;

            case WindowEventID.Restored:
            case WindowEventID.FocusGained:
                {
                    Focused = true;
                    OnFocusChanged?.Invoke(Focused);
                    break;
                }

            case WindowEventID.Minimized:
            case WindowEventID.FocusLost:
                {
                    Focused = false;
                    OnFocusChanged?.Invoke(Focused);
                    break;
                }
        }
    }

    private void handleDropEvent(DropEvent evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Dropfile:
                {
                    string str = new((sbyte*)evt.File);

                    if (str != null)
                        OnDataDropped?.Invoke(new[] { str });

                    break;
                }
        }
    }

    private IMonitor getMonitorFromSDL(int index)
    {
        var modes = Enumerable
            .Range(0, Sdl.GetNumDisplayModes(index))
            .Select(mi =>
            {
                DisplayMode mode = default;

                if (Sdl.GetDisplayMode(index, mi, ref mode) < 0)
                    Sdl.ThrowError();

                int bpp = 0;
                uint rMask = 0;
                uint gMask = 0;
                uint bMask = 0;
                uint aMask = 0;
                Sdl.PixelFormatEnumToMasks(mode.Format, ref bpp, ref rMask, ref gMask, ref bMask, ref aMask);
                return new VideoMode(new Size2(mode.W, mode.H), mode.RefreshRate, bpp);
            })
            .ToArray();

        var rect = new Rectangle<int>();

        if (Sdl.GetDisplayBounds(index, ref rect) > 0)
            Sdl.ThrowError();

        return new SDLMonitor(index, Sdl.GetDisplayNameS(index), new Mathematics.Rectangle(rect.Origin.X, rect.Origin.Y, rect.Size.X, rect.Size.Y), modes);
    }
}
