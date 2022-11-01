// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Windowing;

public interface INativeWindow
{
    /// <summary>
    /// The type of native window.
    /// </summary>
    NativeWindowKind Kind { get; }

    /// <summary>
    /// X11 window information. Only valid if <see cref="NativeWindowKind.X11"/>.
    /// </summary>
    (nint Display, nint Window)? X11 { get; }

    /// <summary>
    /// Cocoa window information. Only valid if <see cref="NativeWindowKind.Cocoa"/>.
    /// </summary>
    nint? Cocoa { get; }

    /// <summary>
    /// Wayland window information. Only valid if <see cref="NativeWindowKind.Wayland"/>.
    /// </summary>
    (nint Display, nint Surface)? Wayland { get; }

    /// <summary>
    /// WinRT window information. Only valid if <see cref="NativeWindowKind.WinRT"/>.
    /// </summary>
    nint? WinRT { get; }

    /// <summary>
    /// UIKit window information. Only valid if <see cref="NativeWindowKind.UIKit"/>.
    /// </summary>
    (nint Window, uint Framebuffer, uint Colorbuffer, uint ResolveFramebuffer)? UIKit { get; }

    /// <summary>
    /// Win32 window information. Only valid if <see cref="NativeWindowKind.Win32"/>.
    /// </summary>
    (nint Hwnd, nint HDC, nint HInstance)? Win32 { get; }

    /// <summary>
    /// Vivante window information. Only valid if <see cref="NativeWindowKind.Vivante"/>.
    /// </summary>
    (nint Display, nint Window)? Vivante { get; }

    /// <summary>
    /// Android window information. Only valid if <see cref="NativeWindowKind.Android"/>.
    /// </summary>
    (nint Window, nint Surface)? Android { get; }
}
