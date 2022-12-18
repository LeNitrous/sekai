// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Windows.Forms;
using Sekai.Windowing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Sekai.Forms;

internal struct FormsNativeWindow : INativeWindow
{
    public NativeWindowKind Kind => NativeWindowKind.Win32;
    public (nint Display, nint Window)? X11 => throw new NotImplementedException();
    public nint? Cocoa => throw new NotImplementedException();
    public (nint Display, nint Surface)? Wayland => throw new NotImplementedException();
    public nint? WinRT => throw new NotImplementedException();
    public (nint Window, uint Framebuffer, uint Colorbuffer, uint ResolveFramebuffer)? UIKit => throw new NotImplementedException();
    public (nint Hwnd, nint HDC, nint HInstance)? Win32 { get; }
    public (nint Display, nint Window)? Vivante => throw new NotImplementedException();
    public (nint Window, nint Surface)? Android => throw new NotImplementedException();

    public FormsNativeWindow(Form form, nint hdc)
    {
        Win32 = new(form.Handle, hdc, PInvoke.GetWindowLong(new HWND(form.Handle), WINDOW_LONG_PTR_INDEX.GWL_HINSTANCE));
    }
}
