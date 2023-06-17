// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Platform.Windowing;

/// <summary>
/// Describes an <see cref="IWindow"/>'s surface.
/// </summary>
public struct SurfaceInfo
{
    /// <summary>
    /// The surface's kind.
    /// </summary>
    public SurfaceKind Kind;

    /// <inheritdoc cref="X11Surface" />
    public X11Surface X11;

    /// <inheritdoc cref="CocoaSurface" />
    public CocoaSurface Cocoa;

    /// <inheritdoc cref="MirSurface" />
    public MirSurface Mir;

    /// <inheritdoc cref="EGLSurface" />
    public EGLSurface EGL;

    /// <inheritdoc cref="WaylandSurface" />
    public WaylandSurface Wayland;

    /// <inheritdoc cref="DirectFramebufferSurface" />
    public DirectFramebufferSurface DirectFramebuffer;

    /// <inheritdoc cref="WinRTSurface" />
    public WinRTSurface WinRT;

    /// <inheritdoc cref="UIKitSurface" />
    public UIKitSurface UIKit;

    /// <inheritdoc cref="Win32Surface" />
    public Win32Surface Win32;

    /// <inheritdoc cref="VivanteSurface" />
    public VivanteSurface Vivante;

    /// <inheritdoc cref="AndroidSurface" />
    public AndroidSurface Android;
}

/// <summary>
/// The X11 surface info.
/// </summary>
/// <param name="Display">The display pointer (Display*).</param>
/// <param name="Window">The window XID (Window).</param>
public readonly record struct X11Surface(nint Display, nuint Window);

/// <summary>
/// The Cocoa surface info.
/// </summary>
/// <param name="Window">The window handle (NSWindow*).</param>
public readonly record struct CocoaSurface(nint Window);

/// <summary>
/// The Mir surface info.
/// </summary>
/// <param name="Surface">The surface handle (MirSurface*).</param>
public readonly record struct MirSurface(nint Surface);

/// <summary>
/// The EGL surface info.
/// </summary>
/// <param name="Display">The EGL display.</param>
/// <param name="Surface">The EGL surface.</param>
public readonly record struct EGLSurface(nint Display, nint Surface);

/// <summary>
/// The Wayland surface info.
/// </summary>
/// <param name="Display">The display handle (wl_display*).</param>
/// <param name="Surface">The surface handle (wl_surface*).</param>
public readonly record struct WaylandSurface(nint Display, nint Surface);

/// <summary>
/// The direct framebuffer surface info.
/// </summary>
/// <param name="Window">The display handle (IDirectFBWindow*).</param>
/// <param name="Surface">The surface handle (IDirectFBSurface*).</param>
public readonly record struct DirectFramebufferSurface(nint Window, nint Surface);

/// <summary>
/// The WinRT surface info.
/// </summary>
/// <param name="Inspectable">The inspectable handle (IInspectable*).</param>
public readonly record struct WinRTSurface(nint Inspectable);

/// <summary>
/// The UIKIt surface info.
/// </summary>
/// <param name="Window">The window pointer (UIWindow*).</param>
/// <param name="FramebufferId">The OpenGL frame buffer object ID.</param>
/// <param name="ColorbufferId">The OpenGL color buffer object ID.</param>
/// <param name="ResolveFramebufferId">The OpenGL resolve frame buffer object ID.</param>
public readonly record struct UIKitSurface(nint Window, uint FramebufferId, uint ColorbufferId, uint ResolveFramebufferId);

/// <summary>
/// The Win32 surface info.
/// </summary>
/// <param name="Window">The window handle (HWND).</param>
/// <param name="Device">The device handle (HDC).</param>
/// <param name="Instance">The instance handle (HINSTANCE).</param>
public readonly record struct Win32Surface(nint Window, nint Device, nint Instance);

/// <summary>
/// The Vivante surface info.
/// </summary>
/// <param name="Display">The display handle (EGLNativeDisplayType).</param>
/// <param name="Window">The window handle (EGLNativeWindowType).</param>
public readonly record struct VivanteSurface(nint Display, nint Window);

/// <summary>
/// The Android surface info.
/// </summary>
/// <param name="Window">The window handle (ANativeWindow*).</param>
/// <param name="Surface">The surface handle (EGLSurface).</param>
public readonly record struct AndroidSurface(nint Window, nint Surface);
