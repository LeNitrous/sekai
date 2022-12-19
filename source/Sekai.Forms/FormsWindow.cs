// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Sekai.Mathematics;
using Sekai.Windowing;
using Sekai.Windowing.OpenGL;
using Windows.UI.ViewManagement;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Sekai.Forms;

internal class FormsWindow : FrameworkObject, IWindow, INativeWindowSource, IOpenGLContextSource
{
    public event Action<Size2>? OnResize;
    public event Action<Point>? OnMoved;
    public event Action<bool>? OnFocusChanged;
    public event Action<string[]>? OnDataDropped;
    public event Action<bool>? OnStateChanged;
    public event Action? OnClose;
    public event Func<bool>? OnCloseRequested;

    public bool Focused { get; private set; }
    public bool Active { get; private set; }
    public IMonitor Monitor => new FormsMonitor(0, Screen.FromControl(form));
    public IEnumerable<IMonitor> Monitors => Screen.AllScreens.Select((s, i) => (IMonitor)new FormsMonitor(i, s));

    public string Title
    {
        get => window.Title;
        set => window.Title = value;
    }

    public Icon Icon
    {
        get => icon;
        set
        {
            if (icon.Equals(value))
                return;

            icon = value;
            form.BeginInvoke(updateWindowIcon);
        }
    }

    public Point Position
    {
        get => new(window.Position.X, window.Position.Y);
        set => window.Move(new(value.X, value.Y));
    }

    public Size2 Size
    {
        get => new(window.Size.Width, window.Size.Height);
        set => window.Resize(new(value.Width, value.Height));
    }

    public Size2 MinimumSize
    {
        get => new(form.MinimumSize.Width, form.MinimumSize.Height);
        set => form.BeginInvoke(() => form.MinimumSize = new(value.Width, value.Height));
    }
    public Size2 MaximumSize
    {
        get => new(form.MaximumSize.Width, form.MaximumSize.Height);
        set => form.BeginInvoke(() => form.MaximumSize = new(value.Width, value.Height));
    }

    public WindowState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;
            updateWindowPresenter();
        }
    }

    public WindowBorder Border
    {
        get => border;
        set
        {
            if (border == value)
                return;

            border = value;
            updateWindowPresenter();
        }
    }

    public bool Visible
    {
        get => window.IsVisible;
        set => form.BeginInvoke(() => form.Visible = value);
    }

    Size2 IView.Size => Size;
    Point IView.Position => Position;

    public INativeWindow Native => native.Value;
    public IOpenGLContext GL => context.Value;

    private Icon icon;
    private WindowState state;
    private WindowBorder border;
    private readonly Lazy<INativeWindow> native;
    private readonly Lazy<IOpenGLContext> context;
    private readonly Form form;
    private readonly AppWindow window;
    private readonly UISettings settings;
    private readonly System.Drawing.Graphics graphics;

    public FormsWindow()
    {
        form = new()
        {
            Visible = false,
            AllowDrop = true,
            BackColor = System.Drawing.Color.Black
        };

        form.Activated += handleFormActivate;
        form.Deactivate += handleFormDeactivate;
        form.GotFocus += handleFormGotFocus;
        form.LostFocus += handleFormLostFocus;
        form.FormClosing += handleFormClosing;
        form.FormClosed += handleFormClosed;

        graphics = form.CreateGraphics();
        nint hdc = graphics.GetHdc();

        native = new(() => new FormsNativeWindow(form, hdc));
        context = new(() => new FormsGLContext((HDC)hdc));

        window = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(form.Handle));
        window.Changed += handleWindowChange;

        settings = new();
        settings.ColorValuesChanged += handleColorChange;

        updateWindowTheme();
    }

    public Point PointToScreen(Point point)
    {
        var p = form.PointToScreen(new System.Drawing.Point(point.X, point.Y));
        return new Point(p.X, p.Y);
    }

    public Point PointToClient(Point point)
    {
        var p = form.PointToClient(new System.Drawing.Point(point.X, point.Y));
        return new Point(p.X, p.Y);
    }

    public void DoEvents()
    {
        while (PeekMessage(out var msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
        {
            if (!GetMessage(out msg, HWND.Null, 0, 0))
            {
                int error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new ApplicationException($"An application exception has occured. Error Code: {error:0x}");
            }

            TranslateMessage(msg);
            DispatchMessage(msg);
        }
    }

    public void Close() => form.BeginInvoke(form.Close);

    private void handleFormActivate(object? sender, EventArgs args) => OnStateChanged?.Invoke(Active = true);
    private void handleFormDeactivate(object? sender, EventArgs args) => OnStateChanged?.Invoke(Active = false);
    private void handleFormGotFocus(object? sender, EventArgs args) => OnFocusChanged?.Invoke(Focused = true);
    private void handleFormLostFocus(object? sender, EventArgs args) => OnFocusChanged?.Invoke(Focused = false);
    private void handleFormClosed(object? sender, EventArgs args) => OnClose?.Invoke();
    private void handleFormClosing(object? sender, CancelEventArgs args) => args.Cancel = OnCloseRequested?.Invoke() ?? false;
    private void handleColorChange(UISettings sender, object args) => form.BeginInvoke(updateWindowTheme);

    private void handleWindowChange(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPositionChange)
            OnMoved?.Invoke(Position);

        if (args.DidSizeChange)
            OnResize?.Invoke(Size);
    }
    private void updateWindowIcon()
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);
        writer.Write(icon.Data.Span);
        form.Icon = new System.Drawing.Icon(memory);
    }

    private void updateWindowPresenter()
    {
        window.SetPresenter
        (
            state == WindowState.Fullscreen
                ? AppWindowPresenterKind.FullScreen
                : AppWindowPresenterKind.Overlapped
        );

        if (window.Presenter is not OverlappedPresenter overlapped)
            return;

        switch (state)
        {
            case WindowState.Minimized:
                overlapped.Minimize();
                break;

            case WindowState.Maximized:
                overlapped.Maximize();
                break;
        }

        overlapped.SetBorderAndTitleBar(border != WindowBorder.Hidden, true);
    }

    private unsafe void updateWindowTheme()
    {
        // See https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-windows-themes
        var color = settings.GetColorValue(UIColorType.Foreground);
        BOOL value = ((5 * color.G) + (2 * color.R) + color.B) > (8 * 128);
        DwmSetWindowAttribute((HWND)form.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &value, (uint)sizeof(BOOL));
    }

    protected override void Destroy()
    {
        graphics.Dispose();
        form.Dispose();
        window.Destroy();
    }
}
