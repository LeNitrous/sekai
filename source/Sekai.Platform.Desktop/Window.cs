// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.InteropServices;
using Sekai.Framework.OpenGL;
using Sekai.Platform.Input;
using Sekai.Platform.Windowing;
using Silk.NET.GLFW;

namespace Sekai.Platform.Desktop;

public sealed unsafe partial class Window : IWindow, IInputSource, IGLContextSource
{
    public bool Exists { get; private set; }

    public SurfaceInfo Surface
    {
        get
        {
            if (info.Kind != SurfaceKind.Unknown)
            {
                return info;
            }

            info = new();

            if (glfw.Context.TryGetProcAddress("glfwGetWin32Window", out nint getHwnd))
            {
                info.Kind |= SurfaceKind.Win32;
                nint hwnd = ((delegate* unmanaged[Cdecl]<WindowHandle*, nint>)getHwnd)(window);
                info.Win32 = new(hwnd, GetDC(hwnd), GetWindowLongPtr(hwnd, -6));
            }

            if (glfw.Context.TryGetProcAddress("glfwGetCocoaWindow", out nint getCocoa))
            {
                info.Kind |= SurfaceKind.Cocoa;
                info.Cocoa = new((nint)((delegate* unmanaged[Cdecl]<WindowHandle*, void*>)getCocoa)(window));
            }

            if (glfw.Context.TryGetProcAddress("glfwGetX11Display", out nint getX11Display) &&
                glfw.Context.TryGetProcAddress("glfwGetX11Window", out nint getX11Window))
            {
                info.Kind |= SurfaceKind.X11;
                info.X11 = new
                (
                    (nint)((delegate* unmanaged[Cdecl]<void*>)getX11Display)(),
                    ((delegate* unmanaged[Cdecl]<WindowHandle*, nuint>)getX11Window)(window)
                );
            }

            if (glfw.Context.TryGetProcAddress("glfwGetWaylandDisplay", out nint getWaylandDisplay) &&
                glfw.Context.TryGetProcAddress("glfwGetWaylandWindow", out nint getWaylandWindow))
            {
                info.Kind |= SurfaceKind.Wayland;
                info.Wayland = new
                (
                    (nint)((delegate* unmanaged[Cdecl]<void*>)getWaylandDisplay)(),
                    (nint)((delegate* unmanaged[Cdecl]<WindowHandle*, void*>)getWaylandWindow)(window)
                );
            }

            if (glfw.Context.TryGetProcAddress("glfwGetEGLDisplay", out nint getEGLDisplay) &&
                glfw.Context.TryGetProcAddress("glfwGetEGLSurface", out nint getEGLSurface))
            {
                info.Kind |= SurfaceKind.Wayland;
                info.EGL = new
                (
                    (nint)((delegate* unmanaged[Cdecl]<void*>)getEGLDisplay)(),
                    (nint)((delegate* unmanaged[Cdecl]<WindowHandle*, void*>)getEGLSurface)(window)
                );
            }

            return info;
        }
    }

    public WindowBorder Border
    {
        get
        {
            if (glfw.GetWindowAttrib(window, WindowAttributeGetter.Resizable))
            {
                return WindowBorder.Resizable;
            }

            return glfw.GetWindowAttrib(window, WindowAttributeGetter.Decorated)
                ? WindowBorder.Fixed
                : WindowBorder.Hidden;
        }
        set
        {
            switch (value)
            {
                case WindowBorder.Fixed:
                    glfw.SetWindowAttrib(window, WindowAttributeSetter.Resizable, false);
                    glfw.SetWindowAttrib(window, WindowAttributeSetter.Decorated, true);
                    break;

                case WindowBorder.Resizable:
                    glfw.SetWindowAttrib(window, WindowAttributeSetter.Resizable, true);
                    glfw.SetWindowAttrib(window, WindowAttributeSetter.Decorated, true);
                    break;

                case WindowBorder.Hidden:
                    glfw.SetWindowAttrib(window, WindowAttributeSetter.Resizable, false);
                    glfw.SetWindowAttrib(window, WindowAttributeSetter.Decorated, false);
                    break;
            }
        }
    }

    public IMonitor Monitor
    {
        get
        {
            glfw.GetWindowPos(window, out int wx, out int wy);
            glfw.GetWindowSize(window, out int ww, out int wh);

            int bestMonitor = 0;
            int bestOverlap = 0;

            var monitors = glfw.GetMonitors(out int monitorCount);

            for (int i = 0; i < monitorCount; i++)
            {
                var mode = glfw.GetVideoMode(monitors[i]);

                glfw.GetMonitorPos(monitors[i], out int mx, out int my);
                int mw = mode->Width;
                int mh = mode->Height;

                int overlap = Math.Max(0, Math.Min(wx + ww, mx + mw) - Math.Max(wx, mx)) * Math.Max(0, Math.Min(wy + wh, my + mh) - Math.Max(wy, my));

                if (bestOverlap < overlap)
                {
                    bestMonitor = i;
                    bestOverlap = overlap;
                }
            }

            var m = glfw.GetVideoMode(monitors[bestMonitor]);
            glfw.GetMonitorPos(monitors[bestMonitor], out int x, out int y);

            var modes = glfw.GetVideoModes(monitors[bestMonitor], out int modeCount);
            var ms = new Windowing.VideoMode[modeCount];

            for (int i = 0; i < modeCount; i++)
            {
                ms[i] = new(new(modes[i].Width, modes[i].Height), modes[i].RefreshRate);
            }

            string name = glfw.GetMonitorName(monitors[bestMonitor]);

            return new Monitor(bestMonitor, name, new(x, y), new(new(m->Width, m->Height), m->RefreshRate), ms);
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

            switch (state = value)
            {
                case WindowState.Normal:
                    glfw.RestoreWindow(window);
                    break;

                case WindowState.Minimized:
                    glfw.IconifyWindow(window);
                    break;

                case WindowState.Maximized:
                    glfw.MaximizeWindow(window);
                    break;

                case WindowState.Fullscreen:
                    {
                        var monitor = Monitor;
                        var monitors = glfw.GetMonitors(out int count);

                        if (monitor.Index > count)
                        {
                            throw new InvalidOperationException();
                        }

                        glfw.SetWindowMonitor(window, monitors[monitor.Index], 0, 0, monitor.Mode.Resolution.Width, monitor.Mode.Resolution.Height, monitor.Mode.RefreshRate);
                        break;
                    }
            }
        }
    }

    public Size Size
    {
        get
        {
            glfw.GetWindowSize(window, out int width, out int height);
            return new(width, height);
        }
        set => glfw.SetWindowSize(window, value.Width, value.Height);
    }

    public Size MinimumSize
    {
        get => minimumSize;
        set
        {
            if (minimumSize.Equals(value))
            {
                return;
            }

            minimumSize = value;
            glfw.SetWindowSizeLimits(window, MinimumSize.Width, MinimumSize.Height, MaximumSize.Width, MaximumSize.Height);
        }
    }

    public Size MaximumSize
    {
        get => maximumSize;
        set
        {
            if (maximumSize.Equals(value))
            {
                return;
            }

            maximumSize = value;
            glfw.SetWindowSizeLimits(window, MinimumSize.Width, MinimumSize.Height, MaximumSize.Width, MaximumSize.Height);
        }
    }

    public Point Position
    {
        get
        {
            glfw.GetWindowPos(window, out int x, out int y);
            return new Point(x, y);
        }
        set => glfw.SetWindowPos(window, value.X, value.Y);
    }

    public bool HasFocus { get; private set; }

    public bool Visible
    {
        get => visible;
        set
        {
            if (visible.Equals(value))
            {
                return;
            }

            if (visible = value)
            {
                glfw.ShowWindow(window);
            }
            else
            {
                glfw.HideWindow(window);
            }
        }
    }

    public string Title
    {
        get => title;
        set
        {
            if (title.Equals(value))
            {
                return;
            }

            glfw.SetWindowTitle(window, title = value);
        }
    }

    public GLContext Context => source ??= new GLContext
    (
        (nint)window,
        glfw.GetProcAddress,
        () => glfw.MakeContextCurrent(window),
        () => glfw.MakeContextCurrent(null),
        () => glfw.SwapBuffers(window),
        glfw.SwapInterval
    );

    public event Action? Closed;
    public event Action? Closing;
    public event Func<bool>? CloseRequested;
    public event Action<Size>? Resized;
    public event Action<Point>? Moved;
    public event Action<bool>? FocusChanged;

#pragma warning disable CS0067 // GLFW does not have mechanisms for resuming and suspending

    public event Action? Resume;
    public event Action? Suspend;

#pragma warning restore CS0067

    private const int default_width = 1280;
    private const int default_height = 720;

    private GlfwCallbacks.WindowCloseCallback? windowCloseCallback;
    private GlfwCallbacks.WindowSizeCallback? windowSizeCallback;
    private GlfwCallbacks.WindowPosCallback? windowPosCallback;
    private GlfwCallbacks.WindowFocusCallback? windowFocusCallback;
    private GlfwCallbacks.KeyCallback? keyCallback;
    private GlfwCallbacks.ScrollCallback? scrollCallback;
    private GlfwCallbacks.CursorPosCallback? cursorPosCallback;
    private GlfwCallbacks.MouseButtonCallback? mouseButtonCallback;

    private Size maximumSize;
    private Size minimumSize;
    private WindowState state;
    private SurfaceInfo info;
    private bool visible = true;
    private string title = string.Empty;
    private bool isDisposed = false;
    private GLContext? source;
    private static bool hasInit = false;
    private readonly Glfw glfw;
    private readonly WindowHandle* window;
    private readonly ConcurrentQueue<EventData> events = new();

    public Window()
    {
        glfw = Glfw.GetApi();

        if (!hasInit)
        {
            if (!glfw.Init())
            {
                throw new InvalidOperationException("Failed to initialize GLFW.");
            }

            hasInit = true;
        }

        glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);

        window = glfw.CreateWindow(default_height, default_height, title, null, null);

        registerCallbacks();

        Exists = true;
    }

    public void Focus()
    {
        glfw.FocusWindow(window);
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

        Exists = false;

        Closed?.Invoke();
    }

    public void DoEvents()
    {
        glfw.PollEvents();
    }

    public Point PointToClient(Point point)
    {
        return new(point.X - Position.X, point.Y - Position.Y);
    }

    public Point PointToScreen(Point point)
    {
        return new(Position.X + point.X, Position.Y + point.Y);
    }

    public bool PumpEvent(out EventData data) => events.TryDequeue(out data);

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

        isDisposed = true;

        unregisterCallbacks();
        glfw.DestroyWindow(window);
        glfw.Terminate();

        GC.SuppressFinalize(this);
    }

    private void registerCallbacks()
    {
        windowCloseCallback = (window) => Close();

        windowFocusCallback = (window, focused) =>
        {
            HasFocus = focused;
            FocusChanged?.Invoke(HasFocus);
        };

        windowPosCallback = (window, x, y) => Moved?.Invoke(new(x, y));

        windowSizeCallback = (window, w, h) => Resized?.Invoke(new(w, h));

        keyCallback = (window, key, code, action, mods) =>
        {
            switch (action)
            {
                case InputAction.Press:
                    events.Enqueue(new(new KeyboardEvent(Key.None, true)));
                    break;

                case InputAction.Release:
                    events.Enqueue(new(new KeyboardEvent(Key.None, false)));
                    break;

                default:
                    break;
            }
        };

        scrollCallback = (window, offsetX, offsetY) =>
        {
            events.Enqueue(new(new MouseScrollEvent(new((float)offsetX, (float)offsetY))));
        };

        cursorPosCallback = (window, x, y) =>
        {
            events.Enqueue(new(new MouseMotionEvent(new((int)x, (int)y))));
        };

        mouseButtonCallback = (window, button, action, mods) =>
        {
            switch (action)
            {
                case InputAction.Press:
                    events.Enqueue(new(new MouseButtonEvent(Input.MouseButton.None, true)));
                    break;

                case InputAction.Release:
                    events.Enqueue(new(new MouseButtonEvent(Input.MouseButton.None, false)));
                    break;

                default:
                    break;
            }
        };

        glfw.SetWindowCloseCallback(window, windowCloseCallback);
        glfw.SetWindowPosCallback(window, windowPosCallback);
        glfw.SetWindowSizeCallback(window, windowSizeCallback);
        glfw.SetWindowFocusCallback(window, windowFocusCallback);
        glfw.SetKeyCallback(window, keyCallback);
        glfw.SetScrollCallback(window, scrollCallback);
        glfw.SetCursorPosCallback(window, cursorPosCallback);
        glfw.SetMouseButtonCallback(window, mouseButtonCallback);
    }

    private void unregisterCallbacks()
    {
        glfw.GcUtility.Unpin(windowCloseCallback);
        glfw.GcUtility.Unpin(windowPosCallback);
        glfw.GcUtility.Unpin(windowSizeCallback);
        glfw.GcUtility.Unpin(windowFocusCallback);
        glfw.GcUtility.Unpin(keyCallback);
        glfw.GcUtility.Unpin(cursorPosCallback);
        glfw.GcUtility.Unpin(mouseButtonCallback);

        windowCloseCallback = null;
        windowPosCallback = null;
        windowSizeCallback = null;
        windowFocusCallback = null;
        keyCallback = null;
        scrollCallback = null;
        cursorPosCallback = null;
        mouseButtonCallback = null;
    }

#pragma warning disable IDE1006

    [LibraryImport("user32.dll")]
    private static partial nint GetDC(nint hwnd);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static partial nint GetWindowLongPtr32(nint hwnd, int index);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static partial nint GetWindowLongPtr64(nint hwnd, int index);

    private static nint GetWindowLongPtr(nint hwnd, int index)
    {
        if (sizeof(nint) == 8)
        {
            return GetWindowLongPtr64(hwnd, index);
        }
        else
        {
            return GetWindowLongPtr32(hwnd, index);
        }
    }

#pragma warning restore IDE1006

}
