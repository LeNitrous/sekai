// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NetEscapades.EnumGenerators;

namespace Sekai.Platform;

/// <summary>
/// Flags determining the surface's identity.
/// </summary>
[Flags, EnumExtensions]
public enum SurfaceFlags
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
    UIKit = 1 << 4,

    /// <summary>
    /// X11.
    /// </summary>
    X11 = 1 << 6,

    /// <summary>
    /// Mir.
    /// </summary>
    Mir = 1 << 8,

    /// <summary>
    /// Wayland.
    /// </summary>
    Wayland = 1 << 10,

    /// <summary>
    /// DirectFramebuffer.
    /// </summary>
    DirectFramebuffer = 1 << 12,

    /// <summary>
    /// Vivante.
    /// </summary>
    Vivante = 1 << 14,

    /// <summary>
    /// Android.
    /// </summary>
    Android = 1 << 16,

    /// <summary>
    /// OpenGL.
    /// </summary>
    OpenGL = 1 << 18,
}
