// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces;

/// <summary>
/// An interface for native surface handles.
/// </summary>
public interface INativeSurface
{
    /// <summary>
    /// The type of native surface.
    /// </summary>
    NativeSurfaceKind Kind { get; }

    /// <summary>
    /// X11 surface information. Only valid if <see cref="NativeSurfaceKind.X11"/>.
    /// </summary>
    (nint Display, nint Window)? X11 { get; }

    /// <summary>
    /// Cocoa surface information. Only valid if <see cref="NativeSurfaceKind.Cocoa"/>.
    /// </summary>
    nint? Cocoa { get; }

    /// <summary>
    /// Wayland surface information. Only valid if <see cref="NativeSurfaceKind.Wayland"/>.
    /// </summary>
    (nint Display, nint Surface)? Wayland { get; }

    /// <summary>
    /// WinRT surface information. Only valid if <see cref="NativeSurfaceKind.WinRT"/>.
    /// </summary>
    nint? WinRT { get; }

    /// <summary>
    /// UIKit surface information. Only valid if <see cref="NativeSurfaceKind.UIKit"/>.
    /// </summary>
    (nint Window, uint Framebuffer, uint Colorbuffer, uint ResolveFramebuffer)? UIKit { get; }

    /// <summary>
    /// Win32 surface information. Only valid if <see cref="NativeSurfaceKind.Win32"/>.
    /// </summary>
    (nint Hwnd, nint HDC, nint HInstance)? Win32 { get; }

    /// <summary>
    /// Vivante surface information. Only valid if <see cref="NativeSurfaceKind.Vivante"/>.
    /// </summary>
    (nint Display, nint Window)? Vivante { get; }

    /// <summary>
    /// Android surface information. Only valid if <see cref="NativeSurfaceKind.Android"/>.
    /// </summary>
    (nint Window, nint Surface)? Android { get; }
}
