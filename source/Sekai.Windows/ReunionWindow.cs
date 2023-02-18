// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Sekai.Windowing;
using Windows.UI.ViewManagement;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;

namespace Sekai.Windows;

/// <summary>
/// A helper class that enables Windows App SDK functionality.
/// </summary>
internal sealed class ReunionWindow : DisposableObject
{
    private readonly HWND hwnd;
    private readonly AppWindow window;
    private readonly UISettings settings;

    public ReunionWindow(Surface surface)
    {
        if (surface is not INativeWindowSource source)
            throw new InvalidOperationException($"Surface must implement {nameof(INativeWindowSource)}.");

        if (source.Native.Kind != NativeWindowKind.Win32 || !source.Native.Win32.HasValue)
            throw new InvalidOperationException($"Native Window must be a Win32 window.");

        hwnd = (HWND)source.Native.Win32.Value.Hwnd;
        window = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow((nint)hwnd));

        settings = new();
        settings.ColorValuesChanged += (sender, args) => updateWindowColors();

        updateWindowColors();
    }

    // See https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-windows-themes
    private unsafe void updateWindowColors()
    {
        var color = settings.GetColorValue(UIColorType.Foreground);
        BOOL value = ((5 * color.G) + (2 * color.R) + color.B) > (8 * 128);
        PInvoke.DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &value, (uint)Unsafe.SizeOf<BOOL>());
    }
}
