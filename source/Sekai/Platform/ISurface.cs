// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform;

/// <summary>
/// Represents a renderable surface. This is usually a control in a user interface.
/// </summary>
public interface ISurface
{
    /// <summary>
    /// The surface flags.
    /// </summary>
    SurfaceFlags Flags { get; }

    /// <summary>
    /// The X11 display pointer (Display*) and the window XID (Window) of the underlying X11 window.
    /// </summary>
    (nint Display, nuint Window)? X11 { get; }

    /// <summary>
    /// The Cocoa window (NSWindow*).
    /// </summary>
    nint? Cocoa { get; }

    /// <summary>
    /// THe Mir surface (MirSurface*).
    /// </summary>
    nint? Mir { get; }

    /// <summary>
    /// The Wayland display pointer (wl_display*) and surface pointer (wl_surface*).
    /// </summary>
    (nint Display, nint Surface)? Wayland { get; }

    /// <summary>
    /// The Direct Framebuffer window pointer (IDirectFBWindow*) and surface pointer (IDirectFBSurface*).
    /// </summary>
    (nint Window, nint Surface)? DirectFramebuffer { get; }

    /// <summary>
    /// The WinRT window's inspectable interface (IInspectable*).
    /// </summary>
    nint? WinRT { get; }

    /// <summary>
    /// The UIKit window pointer (UIWindow*), OpenGL framebuffer object, OpenGL renderbuffer object,
    /// and the reserve color render buffer.
    /// </summary>
    (nint Window, uint Framebuffer, uint Colorbuffer, uint ResolveFramebuffer)? UIKit { get; }

    /// <summary>
    /// The Win32 window handle (HWND), display controller (HDC), and instance (HINSTANCE).
    /// </summary>
    (nint Hwnd, nint HDC, nint HInstance)? Win32 { get; }

    /// <summary>
    /// The Vivante EGL display type (EGLNativeDisplayType) and EGL window type (EGLNativeWindowType).
    /// </summary>
    (nint Display, nint Window)? Vivante { get; }

    /// <summary>
    /// The Android native window pointer (ANativeWindow*) and EGL surface (EGLSurface).
    /// </summary>
    (nint Window, nint Surface)? Android { get; }

    /// <summary>
    /// The OpenGL context source.
    /// </summary>
    IGLContextSource? GL { get; }
}
