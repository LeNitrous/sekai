// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Reflection;

namespace Sekai.Framework;

public static class RuntimeInfo
{
    public static readonly bool IsDebug = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyConfigurationAttribute>()
        ?.Configuration.Contains("Debug", StringComparison.InvariantCultureIgnoreCase) ?? false;

    public static Platform OS { get; }
    public static bool IsUnix => OS != Platform.Windows;
    public static bool SupportsJIT => OS != Platform.iOS;
    public static bool IsDesktop => OS is <= Platform.Windows or > Platform.iOS;
    public static bool IsMobile => OS is < Platform.macOS or >= Platform.Android;
    public static bool IsApple => OS is Platform.iOS or Platform.macOS;

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

    public enum Platform
    {
        Windows = 1,
        Linux = 2,
        macOS = 3,
        iOS = 4,
        Android = 5,
    }
}
