// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.Windows;

/// <summary>
/// A WinRT <see href="https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainpanel?view=windows-app-sdk-1.2">SwapChainPanel</see>.
/// </summary>
public interface IWinRTSwapChainPanel
{
    /// <summary>
    /// The logical DPI of the swapchain panel.
    /// </summary>
    float LogicalDPI { get; }

    /// <summary>
    /// The swapchain panel object.
    /// </summary>
    object Panel { get; }
}
