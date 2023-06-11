// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using System.Drawing;
using System.Linq;
using Sekai.Platform;
using static SDL2.SDL;

namespace Sekai.Desktop;

internal sealed unsafe class Window : IWindow
{
    public event Action? Closed;
    public event Action? Closing;
    public event Action<Size>? Resized;
    public event Action<Point>? Moved;
    public event Func<bool>? CloseRequested;
    public event Action<bool>? FocusChanged;
    public event Action? Resume;
    public event Action? Suspend;

    private bool isDisposed;
    private WindowState state;
    private WindowBorder border;
    private Icon icon = Icon.Empty;
    private readonly nint window;
    private readonly Window? parent;

    public IWindowHost? Parent => (IWindowHost?)parent ?? Monitor;
    public IMonitor? Monitor { get; private set; }
    public ISurface Surface { get; }
    public bool Exists { get; private set; }

    public string Title
    {
        get => SDL_GetWindowTitle(window);
        set => SDL_SetWindowTitle(window, value);
    }

    public Icon Icon
    {
        get => icon;
        set
        {
            if (icon.Equals(value))
            {
                return;
            }

            icon.Dispose();
            icon = value;

            nint surface = IntPtr.Zero;

            fixed (byte* data = icon.Data)
            {
                surface = SDL_CreateRGBSurfaceFrom((nint)data, icon.Size.Width, icon.Size.Height, 32, icon.Size.Width * 4, 0xff, 0xff00, 0xff0000, 0xff000000);
            }

            SDL_SetWindowIcon(window, surface);
            SDL_FreeSurface(surface);
        }
    }

    public bool Visible
    {
        get => ((SDL_WindowFlags)SDL_GetWindowFlags(window)).HasFlag(SDL_WindowFlags.SDL_WINDOW_SHOWN);
        set
        {
            if (value)
            {
                SDL_ShowWindow(window);
            }
            else
            {
                SDL_HideWindow(window);
            }
        }
    }

    public Size Size
    {
        get
        {
            SDL_GetWindowSize(window, out int w, out int h);
            return new(w, h);
        }
        set => SDL_SetWindowSize(window, value.Width, value.Height);
    }

    public Size MinimumSize
    {
        get
        {
            SDL_GetWindowMinimumSize(window, out int w, out int h);
            return new(w, h);
        }
        set => SDL_SetWindowMinimumSize(window, value.Width, value.Height);
    }

    public Size MaximumSize
    {
        get
        {
            SDL_GetWindowMaximumSize(window, out int w, out int h);
            return new(w, h);
        }
        set => SDL_SetWindowMaximumSize(window, value.Width, value.Height);
    }

    public Point Position
    {
        get
        {
            SDL_GetWindowPosition(window, out int x, out int y);
            return new(x, y);
        }
        set => SDL_SetWindowPosition(window, value.X, value.Y);
    }

    public WindowBorder Border
    {
        get => border;
        set
        {
            if (border == value)
            {
                return;
            }

            border = value;
            SDL_SetWindowBordered(window, border != WindowBorder.Hidden ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
            SDL_SetWindowResizable(window, border != WindowBorder.Fixed ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }
    }

    public WindowState State
    {
        get => state;
        set
        {
            if (state == value)
            {
                return;
            }

            state = value;

            if (state == WindowState.Fullscreen)
            {
                // Attempt to do hardware fullscreen first. If it fails, fallback to fake fullscreen.
                if (SDL_SetWindowFullscreen(window, (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0 ||
                    SDL_SetWindowFullscreen(window, (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP) != 0)
                {
                    throw new Exception("Failed to set window to fullscreen.");
                }
            }
            else
            {
                if (SDL_SetWindowFullscreen(window, 0) != 0)
                {
                    throw new Exception("Failed to return window from fullscreen.");
                }

                switch (state)
                {
                    case WindowState.Normal:
                        SDL_ShowWindow(window);
                        break;

                    case WindowState.Minimized:
                        SDL_MinimizeWindow(window);
                        break;

                    case WindowState.Maximized:
                        SDL_MaximizeWindow(window);
                        break;

                    case WindowState.Hidden:
                        SDL_HideWindow(window);
                        break;
                }
            }
        }
    }

    public bool Focus => ((SDL_WindowFlags)SDL_GetWindowFlags(window)).HasFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);

    private static bool hasInit;
    private const int default_width = 1280;
    private const int default_height = 720;
    private const SDL_WindowFlags default_flags = SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_OPENGL;

    public Window()
        : this(SDL_CreateWindow(string.Empty, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, default_width, default_height, default_flags))
    {
    }

    private Window(nint handle, Window parent)
        : this(handle)
    {
        this.parent = parent;
    }

    private Window(nint handle)
    {
        if (!hasInit)
        {
            if (SDL_WasInit(SDL_INIT_EVENTS) == 0 && SDL_Init(SDL_INIT_VIDEO) != 0)
                throw new Exception("Failed to initialize SDL.");

            if (SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE) != 0 &&
                SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3) != 0 &&
                SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2) != 0)
            {
                throw new Exception("Failed to initialize GL.");
            }

            hasInit = true;
        }

        window = handle;
        Exists = true;
        Surface = new Surface(window);

        updateWindowMonitors();
    }

    public Point PointToClient(Point point)
    {
        var position = Position;
        return new(point.X - position.X, point.Y - position.Y);
    }

    public Point PointToScreen(Point point)
    {
        var position = Position;
        return new(point.X + position.X, point.Y + position.Y);
    }

    public IWindow CreateWindow()
    {
        nint child = default;

        switch (Surface!.Flags)
        {
            case SurfaceFlags.Win32:
                child = SDL_CreateWindowFrom(Surface.Win32!.Value.Hwnd);
                break;

            case SurfaceFlags.Cocoa:
                child = SDL_CreateWindowFrom(Surface.Cocoa!.Value);
                break;

            case SurfaceFlags.X11:
                child = SDL_CreateWindowFrom((nint)Surface.X11!.Value.Window);
                break;

            case SurfaceFlags.Wayland:
                child = SDL_CreateWindowFrom(Surface.Wayland!.Value.Surface);
                break;

            default:
                throw new NotSupportedException("The underlying native window does not support creating child windows.");
        }

        return new Window(child, this);
    }

    public void Close()
    {
        if (!Exists)
        {
            return;
        }

        if (!(CloseRequested?.Invoke() ?? true))
        {
            return;
        }

        Closing?.Invoke();

        SDL_DestroyWindow(window);
        Exists = false;

        Closed?.Invoke();
    }

    private const int events_per_peep = 64;
    private readonly SDL_Event[] events = new SDL_Event[events_per_peep];

    public void DoEvents()
    {
        if (Parent is IWindow)
        {
            return;
        }

        if (!Exists)
        {
            return;
        }

        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(Window));
        }

        SDL_PumpEvents();

        int read;

        do
        {
            read = SDL_PeepEvents(events, events_per_peep, SDL_eventaction.SDL_GETEVENT, SDL_EventType.SDL_FIRSTEVENT, SDL_EventType.SDL_LASTEVENT);

            for (int i = 0; i < read; i++)
            {
                handleEvent(events[i]);
            }
        }
        while (read == events_per_peep);
    }

    private void handleEvent(SDL_Event e)
    {
        switch (e.type)
        {
            case SDL_EventType.SDL_QUIT:
            case SDL_EventType.SDL_APP_TERMINATING:
                Close();
                break;

            case SDL_EventType.SDL_DISPLAYEVENT:
                updateWindowMonitors();
                break;

            case SDL_EventType.SDL_WINDOWEVENT:
                handleWindowEvent(e.window);
                break;

            case SDL_EventType.SDL_APP_DIDENTERFOREGROUND:
                Resume?.Invoke();
                break;

            case SDL_EventType.SDL_APP_DIDENTERBACKGROUND:
                Suspend?.Invoke();
                break;
        }
    }

    private void handleWindowEvent(SDL_WindowEvent e)
    {
        switch (e.windowEvent)
        {
            case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                Moved?.Invoke(new(e.data1, e.data2));
                break;

            case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
            case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                Resized?.Invoke(new(e.data1, e.data2));
                break;

            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                FocusChanged?.Invoke(true);
                break;

            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                FocusChanged?.Invoke(false);
                break;
        }
    }

    private void updateWindowMonitors()
    {
        int monitorIndex = SDL_GetWindowDisplayIndex(window);
        var bounds = Rectangle.Empty;

        if (monitorIndex < 0)
        {
            int monitorCount = SDL_GetNumVideoDisplays();

            for (int i = 0; i < monitorCount; i++)
            {
                var size = Size;
                var position = Position;

                if (SDL_GetDisplayUsableBounds(i, out var rect) != 0)
                {
                    continue;
                }

                bounds = new Rectangle(rect.x, rect.y, rect.w, rect.h);

                if (bounds.Contains(new Point(position.X + (size.Width / 2), position.Y + (size.Height / 2))))
                {
                    monitorIndex = i;
                    break;
                }
            }
        }

        if (bounds.IsEmpty)
        {
            if (SDL_GetDisplayBounds(monitorIndex, out var rect) < 0)
            {
                throw new Exception("Failed to retrieve monitor bounds.");
            }

            bounds = new Rectangle(rect.x, rect.y, rect.w, rect.h);
        }

        int modeCount = SDL_GetNumDisplayModes(monitorIndex);

        if (modeCount < 0)
        {
            throw new Exception("Failed to retrieve monitor video modes.");
        }

        var modes = Enumerable
                        .Range(0, modeCount)
                        .Select(modeIndex =>
                        {
                            if (SDL_GetDisplayMode(monitorIndex, modeIndex, out var mode) < 0)
                            {
                                throw new Exception("Failed to retrieve monitor video mode.");
                            }

                            return new VideoMode(new(mode.w, mode.h), mode.refresh_rate);
                        })
                        .ToArray();

        if (SDL_GetWindowDisplayMode(window, out var current) < 0)
        {
            throw new Exception("Failed to retrieve current video mode.");
        }

        string name = SDL_GetDisplayName(monitorIndex);

        Monitor = new Monitor(name, monitorIndex, bounds, new(new(current.w, current.h), current.refresh_rate), modes);
    }

    ~Window()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        Close();
        SDL_Quit();

        isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
