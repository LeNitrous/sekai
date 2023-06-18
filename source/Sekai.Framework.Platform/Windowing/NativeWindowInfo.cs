// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Platform.Windowing;

/// <summary>
/// Describes an <see cref="IWindow"/>'s native info.
/// </summary>
public struct NativeWindowInfo
{
    /// <summary>
    /// The native window's kind.
    /// </summary>
    public NativeWindowKind Kind;

    /// <inheritdoc cref="X11Info" />
    public X11Info X11;

    /// <inheritdoc cref="CocoaInfo" />
    public CocoaInfo Cocoa;

    /// <inheritdoc cref="MirInfo" />
    public MirInfo Mir;

    /// <inheritdoc cref="EGLInfo" />
    public EGLInfo EGL;

    /// <inheritdoc cref="WaylandInfo" />
    public WaylandInfo Wayland;

    /// <inheritdoc cref="DirectFramebufferInfo" />
    public DirectFramebufferInfo DirectFramebuffer;

    /// <inheritdoc cref="WinRTInfo" />
    public WinRTInfo WinRT;

    /// <inheritdoc cref="UIKitInfo" />
    public UIKitInfo UIKit;

    /// <inheritdoc cref="Win32Info" />
    public Win32Info Win32;

    /// <inheritdoc cref="VivanteInfo" />
    public VivanteInfo Vivante;

    /// <inheritdoc cref="AndroidInfo" />
    public AndroidInfo Android;
}

/// <summary>
/// The X11 info.
/// </summary>
/// <param name="Display">The display pointer (Display*).</param>
/// <param name="Window">The window XID (Window).</param>
public readonly record struct X11Info(nint Display, nuint Window);

/// <summary>
/// The Cocoa info.
/// </summary>
/// <param name="Window">The window handle (NSWindow*).</param>
public readonly record struct CocoaInfo(nint Window);

/// <summary>
/// The Mir info.
/// </summary>
/// <param name="Surface">The surface handle (MirSurface*).</param>
public readonly record struct MirInfo(nint Surface);

/// <summary>
/// The EGL info.
/// </summary>
/// <param name="Display">The EGL display.</param>
/// <param name="Surface">The EGL surface.</param>
public readonly record struct EGLInfo(nint Display, nint Surface);

/// <summary>
/// The Wayland info.
/// </summary>
/// <param name="Display">The display handle (wl_display*).</param>
/// <param name="Surface">The surface handle (wl_surface*).</param>
public readonly record struct WaylandInfo(nint Display, nint Surface);

/// <summary>
/// The direct framebuffer info.
/// </summary>
/// <param name="Window">The display handle (IDirectFBWindow*).</param>
/// <param name="Surface">The surface handle (IDirectFBSurface*).</param>
public readonly record struct DirectFramebufferInfo(nint Window, nint Surface);

/// <summary>
/// The WinRT info.
/// </summary>
/// <param name="Inspectable">The inspectable handle (IInspectable*).</param>
public readonly record struct WinRTInfo(nint Inspectable);

/// <summary>
/// The UIKIt info.
/// </summary>
/// <param name="Window">The window pointer (UIWindow*).</param>
/// <param name="FramebufferId">The OpenGL frame buffer object ID.</param>
/// <param name="ColorbufferId">The OpenGL color buffer object ID.</param>
/// <param name="ResolveFramebufferId">The OpenGL resolve frame buffer object ID.</param>
public readonly record struct UIKitInfo(nint Window, uint FramebufferId, uint ColorbufferId, uint ResolveFramebufferId);

/// <summary>
/// The Win32 info.
/// </summary>
/// <param name="Window">The window handle (HWND).</param>
/// <param name="Device">The device handle (HDC).</param>
/// <param name="Instance">The instance handle (HINSTANCE).</param>
public readonly record struct Win32Info(nint Window, nint Device, nint Instance);

/// <summary>
/// The Vivante info.
/// </summary>
/// <param name="Display">The display handle (EGLNativeDisplayType).</param>
/// <param name="Window">The window handle (EGLNativeWindowType).</param>
public readonly record struct VivanteInfo(nint Display, nint Window);

/// <summary>
/// The Android info.
/// </summary>
/// <param name="Window">The window handle (ANativeWindow*).</param>
/// <param name="Surface">The surface handle (EGLSurface).</param>
public readonly record struct AndroidInfo(nint Window, nint Surface);
