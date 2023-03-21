// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.Windows;

/// <summary>
/// An interface for Win32 windows.
/// </summary>
public interface IWin32Window
{
    /// <summary>
    /// The window handle.
    /// </summary>
    nint Handle { get; }

    /// <summary>
    /// The instance handle.
    /// </summary>
    nint Instance { get; }
}
