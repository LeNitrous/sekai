// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Contexts;

/// <summary>
/// The surface's kind.
/// </summary>
public enum NativeWindowKind
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Win32.
    /// </summary>
    Win32,

    /// <summary>
    /// WinRT.
    /// </summary>
    WinRT,

    /// <summary>
    /// Cocoa.
    /// </summary>
    Cocoa,

    /// <summary>
    /// UIKit.
    /// </summary>
    UIKit,

    /// <summary>
    /// X11.
    /// </summary>
    X11,

    /// <summary>
    /// Mir.
    /// </summary>
    Mir,

    /// <summary>
    /// EGL.
    /// </summary>
    EGL,

    /// <summary>
    /// Wayland.
    /// </summary>
    Wayland,

    /// <summary>
    /// DirectFramebuffer.
    /// </summary>
    DirectFramebuffer,

    /// <summary>
    /// Vivante.
    /// </summary>
    Vivante,

    /// <summary>
    /// Android.
    /// </summary>
    Android,
}
