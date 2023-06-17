// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Platform.Windowing;

/// <summary>
/// The surface's kind.
/// </summary>
[Flags]
public enum SurfaceKind
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Win32.
    /// </summary>
    Win32 = 1 << 0,

    /// <summary>
    /// WinRT.
    /// </summary>
    WinRT = 1 << 1,

    /// <summary>
    /// Cocoa.
    /// </summary>
    Cocoa = 1 << 2,

    /// <summary>
    /// UIKit.
    /// </summary>
    UIKit = 1 << 3,

    /// <summary>
    /// X11.
    /// </summary>
    X11 = 1 << 4,

    /// <summary>
    /// Mir.
    /// </summary>
    Mir = 1 << 5,

    /// <summary>
    /// EGL.
    /// </summary>
    EGL = 1 << 6,

    /// <summary>
    /// Wayland.
    /// </summary>
    Wayland = 1 << 7,

    /// <summary>
    /// DirectFramebuffer.
    /// </summary>
    DirectFramebuffer = 1 << 8,

    /// <summary>
    /// Vivante.
    /// </summary>
    Vivante = 1 << 9,

    /// <summary>
    /// Android.
    /// </summary>
    Android = 1 << 10,
}
