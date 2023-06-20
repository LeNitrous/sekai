// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Sekai.Desktop.Input;
using Sekai.Framework.Contexts;
using Sekai.Framework.Input;
using Sekai.Framework.Mathematics;
using Sekai.Framework.Windowing;
using Silk.NET.GLFW;

namespace Sekai.Desktop.Windowing;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
internal sealed unsafe partial class Window : IWindow, IHasIcon, IHasDragDrop, IGLContextSource, IInputContext
{
    public bool Exists { get; private set; }

    public NativeWindowInfo Surface
    {
        get
        {
            if (info.Kind != NativeWindowKind.Unknown)
            {
                return info;
            }

            info = new();

            if (glfw.Context.TryGetProcAddress("glfwGetWin32Window", out nint getHwnd))
            {
                info.Kind |= NativeWindowKind.Win32;
                nint hwnd = ((delegate* unmanaged[Cdecl]<WindowHandle*, nint>)getHwnd)(window);
                info.Win32 = new(hwnd, GetDC(hwnd), GetWindowLongPtr(hwnd, -6));
            }

            if (glfw.Context.TryGetProcAddress("glfwGetCocoaWindow", out nint getCocoa))
            {
                info.Kind |= NativeWindowKind.Cocoa;
                info.Cocoa = new((nint)((delegate* unmanaged[Cdecl]<WindowHandle*, void*>)getCocoa)(window));
            }

            if (glfw.Context.TryGetProcAddress("glfwGetX11Display", out nint getX11Display) &&
                glfw.Context.TryGetProcAddress("glfwGetX11Window", out nint getX11Window))
            {
                info.Kind |= NativeWindowKind.X11;
                info.X11 = new
                (
                    (nint)((delegate* unmanaged[Cdecl]<void*>)getX11Display)(),
                    ((delegate* unmanaged[Cdecl]<WindowHandle*, nuint>)getX11Window)(window)
                );
            }

            if (glfw.Context.TryGetProcAddress("glfwGetWaylandDisplay", out nint getWaylandDisplay) &&
                glfw.Context.TryGetProcAddress("glfwGetWaylandWindow", out nint getWaylandWindow))
            {
                info.Kind |= NativeWindowKind.Wayland;
                info.Wayland = new
                (
                    (nint)((delegate* unmanaged[Cdecl]<void*>)getWaylandDisplay)(),
                    (nint)((delegate* unmanaged[Cdecl]<WindowHandle*, void*>)getWaylandWindow)(window)
                );
            }

            if (glfw.Context.TryGetProcAddress("glfwGetEGLDisplay", out nint getEGLDisplay) &&
                glfw.Context.TryGetProcAddress("glfwGetEGLSurface", out nint getEGLSurface))
            {
                info.Kind |= NativeWindowKind.Wayland;
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

    public IMonitor? Monitor
    {
        get
        {
            var s = Size;
            var p = Position;

            int wx = p.X;
            int wy = p.Y;
            int ww = s.Width;
            int wh = s.Height;
            int bestOverlap = 0;

            IMonitor? bestMonitor = null;

            foreach (var monitor in host.Monitors)
            {
                int mx = monitor.Position.X;
                int my = monitor.Position.Y;
                int mw = monitor.Mode.Resolution.Width;
                int mh = monitor.Mode.Resolution.Height;

                int overlap = Math.Max(0, Math.Min(wx + ww, mx + mw) - Math.Max(wx, mx)) * Math.Max(0, Math.Min(wy + wh, my + mh) - Math.Max(wy, my));

                if (bestOverlap < overlap)
                {
                    bestMonitor = monitor;
                    bestOverlap = overlap;
                }
            }

            return bestMonitor;
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

                        if (monitor is null || monitor.Index > count)
                        {
                            throw new InvalidOperationException("Failed to set state as fullscreen.");
                        }

                        glfw.SetWindowMonitor(window, ((Monitor)monitor).Handle, 0, 0, monitor.Mode.Resolution.Width, monitor.Mode.Resolution.Height, monitor.Mode.RefreshRate);
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

    public string Class { get; init; } = "Sekai";

    public IEnumerable<IInputDevice> Devices => devices;

    public event Action? Closed;
    public event Action? Closing;
    public event Func<bool>? CloseRequested;
    public event Action<Size>? Resized;
    public event Action<Point>? Moved;
    public event Action<bool>? FocusChanged;
    public event Action<WindowState>? StateChanged;
    public event Action<string[]>? Dropped;

#pragma warning disable CS0067 // Keyboard and mouse are always connected.

    public event Action<IInputDevice, bool>? ConnectionChanged;

#pragma warning restore CS0067

    private const int default_width = 1280;
    private const int default_height = 720;

    private readonly GlfwCallbacks.KeyCallback? keyboardKeyCallback;
    private readonly GlfwCallbacks.DropCallback? dropCallback;
    private readonly GlfwCallbacks.ScrollCallback? mouseScrollCallback;
    private readonly GlfwCallbacks.CursorPosCallback? mouseMotionCallback;
    private readonly GlfwCallbacks.MouseButtonCallback? mouseButtonCallback;
    private readonly GlfwCallbacks.WindowCloseCallback? windowCloseCallback;
    private readonly GlfwCallbacks.WindowSizeCallback? windowSizeCallback;
    private readonly GlfwCallbacks.WindowPosCallback? windowPosCallback;
    private readonly GlfwCallbacks.WindowFocusCallback? windowFocusCallback;
    private readonly GlfwCallbacks.WindowIconifyCallback? windowIconifyCallback;
    private readonly GlfwCallbacks.WindowMaximizeCallback? windowMaximizeCallback;

    private Size maximumSize;
    private Size minimumSize;
    private WindowState state;
    private NativeWindowInfo info;
    private bool visible;
    private string title = string.Empty;
    private bool isDisposed;
    private GLContext? source;
    private readonly Host host;
    private readonly Glfw glfw;
    private readonly WindowHandle* window;
    private readonly IInputDevice[] devices = new IInputDevice[2];
    private readonly Dictionary<int, IController> controllers = new();

    public Window(Host host, Glfw glfw)
    {
        this.glfw = glfw;
        this.host = host;

        glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
        glfw.WindowHint(WindowHintBool.Visible, false);
        glfw.WindowHintString((int)WindowHintString.X11ClassName, Class);
        glfw.WindowHintString((int)WindowHintString.X11InstanceName, Class);

        window = glfw.CreateWindow(default_height, default_height, title, null, null);

        var mouse = new Mouse(glfw, window);
        devices[0] = mouse;

        var keyboard = new Keyboard();
        devices[1] = keyboard;

        glfw.SetDropCallback(window, dropCallback = handleWindowDragDrop);
        glfw.SetKeyCallback(window, keyboardKeyCallback = keyboard.HandleKeyboardKey);
        glfw.SetScrollCallback(window, mouseScrollCallback = mouse.HandleMouseScroll);
        glfw.SetCursorPosCallback(window, mouseMotionCallback = mouse.HandleMouseMotion);
        glfw.SetMouseButtonCallback(window, mouseButtonCallback = mouse.HandleMouseButton);
        glfw.SetWindowCloseCallback(window, windowCloseCallback = handleWindowClose);
        glfw.SetWindowPosCallback(window, windowPosCallback = handleWindowMotion);
        glfw.SetWindowSizeCallback(window, windowSizeCallback = handleWindowResize);
        glfw.SetWindowFocusCallback(window, windowFocusCallback = handleWindowFocus);
        glfw.SetWindowIconifyCallback(window, windowIconifyCallback = handleWindowMinimize);
        glfw.SetWindowMaximizeCallback(window, windowMaximizeCallback = handleWindowMaximize);

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

    public void SetWindowIcon(ReadOnlySpan<Icon> icons)
    {
        if (icons == null)
        {
            glfw.SetWindowIcon(window, 0, null);
        }
        else
        {
            var images = stackalloc Image[icons.Length];

            int offset = 0;
            int length = 0;

            for (int i = 0; i < icons.Length; i++)
            {
                length += icons[i].Pixels.Length;
            }

            // We could have just placed the stackalloc call inside the loop.
            // However the compiler complains about a potential stack overflow.
            // Instead, we sum up the lengths of all pixel lengths and store it
            // in one span instead iterating it over an offset and length as we go.
            Span<byte> memory = stackalloc byte[length];

            for (int i = 0; i < icons.Length; i++)
            {
                var icon = icons[i];
                var span = memory[offset..(offset + icon.Pixels.Length)];

                images[i] = new()
                {
                    Width = icon.Size.Width,
                    Height = icon.Size.Height,
                    Pixels = (byte*)Unsafe.AsPointer(ref span[0]),
                };

                icon.Pixels.Span.CopyTo(span);
                offset += icon.Pixels.Length;
            }

            glfw.SetWindowIcon(window, icons.Length, images);
        }
    }

    private void handleWindowClose(WindowHandle* window)
    {
        Close();
    }

    private void handleWindowFocus(WindowHandle* window, bool focused)
    {
        FocusChanged?.Invoke(HasFocus = focused);
    }

    private void handleWindowMotion(WindowHandle* window, int x, int y)
    {
        Moved?.Invoke(new(x, y));
    }

    private void handleWindowResize(WindowHandle* window, int w, int h)
    {
        Resized?.Invoke(new(w, h));
    }

    private void handleWindowMinimize(WindowHandle* window, bool minimized)
    {
        if (minimized)
        {
            StateChanged?.Invoke(State);
        }
    }

    private void handleWindowMaximize(WindowHandle* window, bool maximized)
    {
        if (maximized)
        {
            StateChanged?.Invoke(State);
        }
    }

    private unsafe void handleWindowDragDrop(WindowHandle* window, int count, nint content)
    {
        string[] paths = new string[count];

        for (int i = 0; i < count; i++)
        {
            var span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(((char**)content)[i]);
            paths[i] = new string(span);
        }

        Dropped?.Invoke(paths);
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

        isDisposed = true;

        glfw.SetDropCallback(window, null);
        glfw.SetKeyCallback(window, null);
        glfw.SetCursorPosCallback(window, null);
        glfw.SetScrollCallback(window, null);
        glfw.SetMouseButtonCallback(window, null);
        glfw.SetWindowCloseCallback(window, null);
        glfw.SetWindowPosCallback(window, null);
        glfw.SetWindowSizeCallback(window, null);
        glfw.SetWindowFocusCallback(window, null);
        glfw.SetWindowIconifyCallback(window, null);
        glfw.SetWindowMaximizeCallback(window, null);

        glfw.DestroyWindow(window);

        GC.SuppressFinalize(this);
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
