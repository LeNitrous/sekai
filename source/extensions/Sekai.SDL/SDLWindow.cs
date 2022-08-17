// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sekai.Framework.Windowing;
using static SDL2.SDL;

namespace Sekai.SDL;

public class SDLWindow : SDLView, IWindow
{
    public bool Focused { get; private set; }
    public IMonitor Monitor => getMonitorFromSDL(SDL_GetWindowDisplayIndex(Window));
    public IEnumerable<IMonitor> Monitors => Enumerable.Range(0, SDL_GetNumVideoDisplays()).Select(getMonitorFromSDL);
    public event Action<Size> OnResize = null!;
    public event Action<Point> OnMoved = null!;
    public event Action<string[]> OnDataDropped = null!;
    public event Action<bool> OnFocusChanged = null!;

    private string title = string.Empty;

    public string Title
    {
        get => title;
        set
        {
            if (title == value)
                return;

            title = value;
            SDL_SetWindowTitle(Window, title);
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
                SDL_ShowWindow(Window);
            }
            else
            {
                SDL_HideWindow(Window);
            }
        }
    }

    private Icon icon;

    public Icon Icon
    {
        get => icon;
        set
        {
            if (icon == value)
                return;

            icon = value;
            updateWindowIcon();
        }
    }

    public Point Position
    {
        get
        {
            SDL_GetWindowPosition(Window, out int x, out int y);
            return new Point(x, y);
        }
        set => SDL_SetWindowPosition(Window, value.X, value.Y);
    }

    public Size Size
    {
        get
        {
            SDL_GetWindowSize(Window, out int w, out int h);
            return new Size(w, h);
        }
        set => SDL_SetWindowSize(Window, value.Width, value.Height);
    }

    public Size MaximumSize
    {
        get
        {
            SDL_GetWindowMaximumSize(Window, out int w, out int h);
            return new Size(w, h);
        }
        set => SDL_SetWindowMaximumSize(Window, value.Width, value.Height);
    }

    public Size MinimumSize
    {
        get
        {
            SDL_GetWindowMinimumSize(Window, out int w, out int h);
            return new Size(w, h);
        }
        set => SDL_SetWindowMinimumSize(Window, value.Width, value.Height);
    }

    public WindowState State
    {
        get => ((SDL_WindowFlags)SDL_GetWindowFlags(Window)).ToWindowState();
        set
        {
            switch (value)
            {
                case WindowState.Normal:
                    SDL_RestoreWindow(Window);
                    break;

                case WindowState.Minimized:
                    SDL_MinimizeWindow(Window);
                    break;

                case WindowState.Maximized:
                    SDL_MaximizeWindow(Window);
                    break;

                case WindowState.Fullscreen:
                    {
                        if (SDL_SetWindowFullscreen(Window, (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) > 0)
                            throw new InvalidOperationException($"Failed to enter fullscreen: {SDL_GetError()}");

                        break;
                    }
            }
        }
    }

    public WindowBorder Border
    {
        get => ((SDL_WindowFlags)SDL_GetWindowFlags(Window)).ToWindowBorder();
        set
        {
            switch (value)
            {
                case WindowBorder.Resizable:
                    {
                        SDL_SetWindowBordered(Window, SDL_bool.SDL_TRUE);
                        SDL_SetWindowResizable(Window, SDL_bool.SDL_TRUE);
                        break;
                    }

                case WindowBorder.Fixed:
                    {
                        SDL_SetWindowBordered(Window, SDL_bool.SDL_TRUE);
                        SDL_SetWindowResizable(Window, SDL_bool.SDL_FALSE);
                        break;
                    }

                case WindowBorder.Hidden:
                    {
                        SDL_SetWindowBordered(Window, SDL_bool.SDL_FALSE);
                        SDL_SetWindowResizable(Window, SDL_bool.SDL_FALSE);
                        break;
                    }

            }
        }
    }

    protected override void ProcessEvent(SDL_Event evt)
    {
        base.ProcessEvent(evt);

        switch (evt.type)
        {
            case SDL_EventType.SDL_WINDOWEVENT:
                handleWindowEvent(evt.window);
                break;

            case SDL_EventType.SDL_DROPFILE:
            case SDL_EventType.SDL_DROPTEXT:
            case SDL_EventType.SDL_DROPBEGIN:
            case SDL_EventType.SDL_DROPCOMPLETE:
                handleDropEvent(evt.drop);
                break;
        }
    }

    private void handleWindowEvent(SDL_WindowEvent evt)
    {
        switch (evt.windowEvent)
        {
            case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                OnMoved?.Invoke(Position);
                break;

            case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                OnResize?.Invoke(Size);
                break;

            case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                {
                    Focused = true;
                    OnFocusChanged?.Invoke(Focused);
                    break;
                }

            case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                {
                    Focused = false;
                    OnFocusChanged?.Invoke(Focused);
                    break;
                }
        }
    }

    private void handleDropEvent(SDL_DropEvent evt)
    {
        switch (evt.type)
        {
            case SDL_EventType.SDL_DROPFILE:
                {
                    string str = UTF8_ToManaged(evt.file, true);

                    if (str != null)
                        OnDataDropped?.Invoke(new[] { str });

                    break;
                }
        }
    }

    private unsafe void updateWindowIcon()
    {
        nint surface = IntPtr.Zero;

        fixed (byte* ptr = icon.Data.Span)
            surface = SDL_CreateRGBSurfaceFrom((IntPtr)ptr, icon.Width, icon.Height, 32, icon.Width * 4, 0xff000000, 0x00ff0000, 0x0000ff00, 0x000000ff);

        SDL_SetWindowIcon(Window, surface);
        SDL_FreeSurface(surface);
    }

    private static IMonitor getMonitorFromSDL(int index)
    {
        var modes = Enumerable
            .Range(0, SDL_GetNumDisplayModes(index))
            .Select(mi =>
            {
                if (SDL_GetDisplayMode(index, mi, out var mode) > 0)
                {
                    throw new InvalidOperationException($"An SDL error has occured: {SDL_GetError()}");
                }

                SDL_PixelFormatEnumToMasks(mode.format, out int bpp, out _, out _, out _, out _);
                return new VideoMode(new Size(mode.w, mode.h), mode.refresh_rate, bpp);
            })
            .ToArray();

        if (SDL_GetDisplayBounds(index, out var rect) > 0)
        {
            throw new InvalidOperationException($"An SDL error has occured: {SDL_GetError()}");
        }

        return new SDLMonitor(index, SDL_GetDisplayName(index), new Rectangle(rect.x, rect.y, rect.w, rect.h), modes);
    }
}
