// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces;

/// <summary>
/// An enumeration of native window kinds.
/// </summary>
public enum NativeSurfaceKind
{
    Unknown = 0,
    OS2,
    Haiku,
    DirectFramebuffer,
    Win32,
    X11,
    Cocoa,
    UIKit,
    Wayland,
    WinRT,
    Android,
    Vivante,
}
