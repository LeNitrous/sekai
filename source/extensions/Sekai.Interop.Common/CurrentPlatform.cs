// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Contains code from MonoGame.
// Copyright 2021 the MonoGame Team.
// Licensed under the Microsoft Public License and MIT.
// See https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt.
using System;
using System.Runtime.InteropServices;

namespace Sekai.Interop.Common;
public enum OS
{
    Windows,
    Linux,
    MacOSX,
    Unknown
}

public static class CurrentPlatform
{
#pragma warning disable IDE1006 // Naming Styles
    private static bool _init = false;
#pragma warning restore IDE1006 // Naming Styles
    private static OS os;

    [DllImport("libc")]
    private static extern int uname(IntPtr buf);

    private static void init()
    {
        if (_init)
            return;

        var pid = Environment.OSVersion.Platform;

        switch (pid)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                os = OS.Windows;
                break;
            case PlatformID.MacOSX:
                os = OS.MacOSX;
                break;
            case PlatformID.Unix:
                os = OS.MacOSX;

                var buf = IntPtr.Zero;

                try
                {
                    buf = Marshal.AllocHGlobal(8192);

                    if (uname(buf) == 0 && Marshal.PtrToStringAnsi(buf) == "Linux")
                        os = OS.Linux;
                }
                catch
                {
                }
                finally
                {
                    if (buf != IntPtr.Zero)
                        Marshal.FreeHGlobal(buf);
                }

                break;
            // we don't support the following so we'll just mark them unknown
            case PlatformID.Xbox:
                os = OS.Unknown;
                break;
            case PlatformID.Other:
                os = OS.Unknown;
                break;
            default:
                os = OS.Unknown;
                break;
        }

        _init = true;
    }

    public static OS OS
    {
        get
        {
            init();
            return os;
        }
    }

    public static string Rid
    {
        get
        {
            if (OS == OS.Windows && Environment.Is64BitProcess)
                return "win-x64";
            else if (OS == OS.Windows && !Environment.Is64BitProcess)
                return "win-x86";
            else if (OS == OS.Linux)
                return "linux-x64";
            else if (OS == OS.MacOSX)
                return "osx";
            else
                return "unknown";
        }
    }
}
