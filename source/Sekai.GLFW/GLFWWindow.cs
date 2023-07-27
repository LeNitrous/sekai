// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Sekai.Contexts;
using Sekai.Input;
using Sekai.Mathematics;
using Sekai.Windowing;
using Silk.NET.GLFW;

namespace Sekai.GLFW;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("osx")]
internal sealed unsafe partial class GLFWWindow : IWindow, IGLContextSource, IInputSource
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

    public Windowing.Monitor Monitor { get; private set; }

    public IEnumerable<Windowing.Monitor> Monitors { get; }

    public IWindowHost Owner => owner ?? (IWindowHost)Monitor;

    public IEnumerable<IInputDevice> Devices  { get; }

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
                    glfw.SetWindowMonitor(window, ((GLFWMonitor)Monitor).Handle, 0, 0, Monitor.Mode.Resolution.Width, Monitor.Mode.Resolution.Height, Monitor.Mode.RefreshRate);
                    break;
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

    public event Action? Tick;
    public event Action? Closed;
    public event Action? Closing;

#pragma warning disable CS0067

    public event Action? Suspend;
    public event Action? Resumed;

#pragma warning restore CS0067

    public event Func<bool>? CloseRequested;
    public event Action<Size>? Resized;
    public event Action<Point>? Moved;
    public event Action<bool>? FocusChanged;
    public event Action<WindowState>? StateChanged;
    public event Action<string[]>? Dropped;
    public event Action<IInputDevice, bool>? ConnectionChanged;

    private const int default_width = 1280;
    private const int default_height = 720;

    private readonly GlfwCallbacks.MonitorCallback? monitorCallback;
    private readonly GlfwCallbacks.JoystickCallback? joystickCallback;
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
    private GLFWMonitor[] monitors = new GLFWMonitor[1];
    private GLFWController[] controllers = Array.Empty<GLFWController>();
    private readonly GLFWWindow? owner;
    private readonly bool owns;
    private readonly Glfw glfw;
    private readonly WindowHandle* window;
    private readonly GLFWMouse mouse;
    private readonly GLFWKeyboard keyboard;

    public GLFWWindow()
        : this(Glfw.GetApi(), true)
    {
    }

    public GLFWWindow(Glfw glfw)
        : this(glfw, false)
    {
    }

    private GLFWWindow(GLFWWindow owner)
        : this(owner.glfw, false)
    {
        this.owner = owner;
    }

    private GLFWWindow(Glfw glfw, bool owns)
    {
        this.glfw = glfw;
        this.owns = owns;

        if (owns)
        {
            if (!glfw.Init())
            {
                throw new InvalidOperationException("Failed to initialize GLFW.");
            }

            glfw.SetMonitorCallback(monitorCallback = handleMonitorChanged);
            glfw.SetJoystickCallback(joystickCallback = handleControllersChanged);
        }

        glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
        glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
        glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
        glfw.WindowHint(WindowHintBool.Visible, false);

        window = glfw.CreateWindow(default_height, default_height, title, null, null);
        mouse = new GLFWMouse(glfw, window);
        Devices = new DeviceEnumerable(this);
        keyboard = new GLFWKeyboard();
        Monitors = new MonitorEnumerable(this);

        refreshMonitors();

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

    public void Run()
    {
        while (Exists)
        {
            if (owns)
            {
                glfw.PollEvents();

                for (int i = 0; i < controllers.Length; i++)
                {
                    var controller = controllers[i];

                    if (controller is null)
                    {
                        continue;
                    }

                    controller.Update();
                }
            }

            Tick?.Invoke();
        }
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

    public IWindow CreateWindow()
    {
        return new GLFWWindow(this);
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
        refreshCurrentMonitor();
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

    private void handleControllersChanged(int id, ConnectedState state)
    {
        if (state is ConnectedState.Connected)
        {
            if (controllers.Length <= id)
            {
                Array.Resize(ref controllers, id + 1);
            }

            if (glfw.JoystickIsGamepad(id))
            {
                controllers[id] = new GLFWGamepad(glfw, id);
            }
            else
            {
                controllers[id] = new GLFWJoystick(glfw, id);
            }

            ConnectionChanged?.Invoke(controllers[id], true);
        }
        else
        {
            if (id >= controllers.Length)
            {
                return;
            }

            if (controllers[id] is null)
            {
                return;
            }

            var controller = controllers[id];
            controllers[id] = null!;

            ConnectionChanged?.Invoke(controller, false);
        }
    }

    private void handleMonitorChanged(Silk.NET.GLFW.Monitor* m, ConnectedState state)
    {
        refreshMonitors();
    }

    [MemberNotNull(nameof(Monitor))]
    private void refreshMonitors()
    {
        var monitorHandles = glfw.GetMonitors(out int monitorCount);

        if (monitors.Length <= monitorCount)
        {
            Array.Resize(ref monitors, monitorCount);

            for (int i = 0; i < monitorCount; i++)
            {
                if (monitors[i] is null)
                {
                    monitors[i] = new GLFWMonitor(glfw, i);
                }

                monitors[i].Handle = monitorHandles[i];
            }
        }
        else
        {
            for (int i = 0; i < monitorCount; i++)
            {
                monitors[^i] = null!;
            }
        }

        refreshCurrentMonitor();
    }

    [MemberNotNull(nameof(Monitor))]
    private void refreshCurrentMonitor()
    {
        var s = Size;
        var p = Position;

        int wx = p.X;
        int wy = p.Y;
        int ww = s.Width;
        int wh = s.Height;
        int bestMonitor = 0;
        int bestOverlap = 0;

        for (int i = 0; i < monitors.Length; i++)
        {
            var monitor = monitors[i];

            if (monitor is null)
            {
                continue;
            }

            int mx = monitor.Position.X;
            int my = monitor.Position.Y;
            int mw = monitor.Mode.Resolution.Width;
            int mh = monitor.Mode.Resolution.Height;

            int overlap = Math.Max(0, Math.Min(wx + ww, mx + mw) - Math.Max(wx, mx)) * Math.Max(0, Math.Min(wy + wh, my + mh) - Math.Max(wy, my));

            if (bestOverlap < overlap)
            {
                bestMonitor = i;
                bestOverlap = overlap;
            }
        }

        Monitor = monitors[bestMonitor];
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

        if (owns)
        {
            glfw.Terminate();
        }

        GC.SuppressFinalize(this);
    }

    private readonly struct MonitorEnumerable : IEnumerable<GLFWMonitor>
    {
        private readonly GLFWWindow window;

        public MonitorEnumerable(GLFWWindow window)
        {
            this.window = window;
        }

        public IEnumerator<GLFWMonitor> GetEnumerator()
        {
            return new MonitorEnumerator(window);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct MonitorEnumerator : IEnumerator<GLFWMonitor>
        {
            public GLFWMonitor Current { get; private set; } = null!;

            private int position;
            private readonly GLFWWindow window;

            public MonitorEnumerator(GLFWWindow window)
            {
                this.window = window;
            }

            public bool MoveNext()
            {
                if (position >= window.monitors.Length)
                {
                    return false;
                }

                Current = window.monitors[position];

                if (Current is null)
                {
                    return false;
                }

                position++;
                return true;
            }

            public void Reset()
            {
                position = 0;
            }

            public readonly void Dispose()
            {
            }

            readonly object IEnumerator.Current => Current;
        }
    }

    private readonly struct DeviceEnumerable : IEnumerable<IInputDevice>
    {
        private readonly GLFWWindow window;

        public DeviceEnumerable(GLFWWindow window)
        {
            this.window = window;
        }

        public IEnumerator<IInputDevice> GetEnumerator()
        {
            return new DeviceEnumerator(window);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct DeviceEnumerator : IEnumerator<IInputDevice>
        {
            public IInputDevice Current { get; private set; } = null!;

            private int position;
            private readonly GLFWWindow window;

            public DeviceEnumerator(GLFWWindow window)
            {
                this.window = window;
            }

            public bool MoveNext()
            {
                if (position == 0)
                {
                    Current = window.mouse;
                }

                if (position == 1)
                {
                    Current = window.keyboard;
                }

                if (position >= 2)
                {
                    int index = position - 2;

                    if (index >= window.controllers.Length)
                    {
                        return false;
                    }

                    if (window.controllers[index] is null)
                    {
                        return false;
                    }

                    Current = window.controllers[index];
                }

                position++;
                return true;
            }

            public void Reset()
            {
                position = 0;
            }

            public readonly void Dispose()
            {
            }

            readonly object IEnumerator.Current => Current;
        }
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
