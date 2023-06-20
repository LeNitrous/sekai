// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.Versioning;

namespace Sekai.Framework;

/// <summary>
/// Provides context of the current runtime environment.
/// </summary>
public static class RuntimeInfo
{
    /// <summary>
    /// Whether execution is currently in debug mode.
    /// </summary>
    public static readonly bool IsDebug =
#if DEBUG
    true
#else
    false
#endif
    ;

    /// <summary>
    /// Gets the operating system currently in execution.
    /// </summary>
    public static Platform OS { get; }

    /// <summary>
    /// Whether the environment is a UNIX system or not.
    /// </summary>
    [SupportedOSPlatformGuard("linux")]
    [SupportedOSPlatformGuard("osx")]
    [SupportedOSPlatformGuard("android")]
    [SupportedOSPlatformGuard("maccatalyst")]
    public static bool IsUnix => OS is > Platform.Windows and < Platform.Browser;

    /// <summary>
    /// Whether the environment is desktop-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    [SupportedOSPlatformGuard("linux")]
    [SupportedOSPlatformGuard("osx")]
    public static bool IsDesktop => OS is >= Platform.Windows and < Platform.iOS;

    /// <summary>
    /// Whether the enivornment is mobile-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("maccatalyst")]
    [SupportedOSPlatformGuard("android")]
    public static bool IsMobile => OS is > Platform.macOS and <= Platform.Android;

    /// <summary>
    /// Whether the environment is Apple-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("osx")]
    [SupportedOSPlatformGuard("maccatalyst")]
    public static bool IsApple => OS is Platform.iOS or Platform.macOS;

    /// <summary>
    /// Whether the environment is Windows or not.
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    public static bool IsWindows => OS is Platform.Windows;

    /// <summary>
    /// Whether the environment is browser-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("browser")]
    public static bool IsBrowser => OS is Platform.Browser;

    /// <summary>
    /// Whether the environment is linux-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("linux")]
    public static bool IsLinux => OS is Platform.Linux;

    /// <summary>
    /// Whether the environment is OSX-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("osx")]
    public static bool IsMacOS => OS is Platform.macOS;

    /// <summary>
    /// Whether the environment is iOS-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("maccatalyst")]
    public static bool IsIOS => OS is Platform.iOS;

    /// <summary>
    /// Whether the environment is Android-based or not.
    /// </summary>
    [SupportedOSPlatformGuard("android")]
    public static bool IsAndroid => OS is Platform.Android;

    static RuntimeInfo()
    {
        if (OperatingSystem.IsWindows())
            OS = Platform.Windows;

        if (OperatingSystem.IsLinux())
            OS = Platform.Linux;

        if (OperatingSystem.IsMacOS())
            OS = Platform.macOS;

        if (OperatingSystem.IsAndroid())
            OS = Platform.Android;

        if (OperatingSystem.IsIOS())
            OS = Platform.iOS;
    }

    /// <summary>
    /// The operating system platform.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Windows.
        /// </summary>
        Windows = 1,

        /// <summary>
        /// Linux.
        /// </summary>
        Linux = 2,

        /// <summary>
        /// MacOS.
        /// </summary>
        macOS = 3,

        /// <summary>
        /// iOS.
        /// </summary>
        iOS = 4,

        /// <summary>
        /// Android.
        /// </summary>
        Android = 5,

        /// <summary>
        /// Browser.
        /// </summary>
        Browser = 6,
    }
}
