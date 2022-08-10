// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Contains code from MonoGame.
// Copyright 2021 the MonoGame Team.
// Licensed under the Microsoft Public License and MIT.
// See https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt.
using Sekai.Interop.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sekai.Interop.Common;
public class FuncLoader
{
    private class Windows
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryW(string lpszLib);
    }

    private class Linux
    {
        [DllImport("libdl.so.2")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("libdl.so.2")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);
    }

    private class OSX
    {
        [DllImport("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);
    }

    private const int rtld_lazy = 0x0001;

    public static IntPtr LoadLibraryExt(string libname)
    {
        var ret = IntPtr.Zero;
        string? assemblyLocation = Path.GetDirectoryName(typeof(FuncLoader).Assembly.Location) ?? "./";

        // Try .NET Framework / mono locations
        if (CurrentPlatform.OS == OS.MacOSX)
        {
            ret = LoadLibrary(Path.Combine(assemblyLocation, libname));

            // Look in Frameworks for .app bundles
            if (ret == IntPtr.Zero)
                ret = LoadLibrary(Path.Combine(assemblyLocation, "..", "Frameworks", libname));
        }
        else
        {
            if (Environment.Is64BitProcess)
                ret = LoadLibrary(Path.Combine(assemblyLocation, "x64", libname));
            else
                ret = LoadLibrary(Path.Combine(assemblyLocation, "x86", libname));
        }

        // Try .NET Core development locations
        if (ret == IntPtr.Zero)
            ret = LoadLibrary(Path.Combine(assemblyLocation, "runtimes", CurrentPlatform.Rid, "native", libname));

        // Try current folder (.NET Core will copy it there after publish)
        if (ret == IntPtr.Zero)
            ret = LoadLibrary(Path.Combine(assemblyLocation, libname));

        // Try alternate way of checking current folder
        // assemblyLocation is null if we are inside macOS app bundle
        if (ret == IntPtr.Zero)
            ret = LoadLibrary(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, libname));

        // Try loading system library
        if (ret == IntPtr.Zero)
            ret = LoadLibrary(libname);

        // Welp, all failed, PANIC!!!
        if (ret == IntPtr.Zero)
            throw new Exception("Failed to load library: " + libname);

        return ret;
    }

    public static IntPtr LoadLibrary(string libname)
    {
        if (CurrentPlatform.OS == OS.Windows)
            return Windows.LoadLibraryW(libname);

        if (CurrentPlatform.OS == OS.MacOSX)
            return OSX.dlopen(libname, rtld_lazy);

        return Linux.dlopen(libname, rtld_lazy);
    }

    public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
    {
        var ret = IntPtr.Zero;

        if (CurrentPlatform.OS == OS.Windows)
            ret = Windows.GetProcAddress(library, function);
        else if (CurrentPlatform.OS == OS.MacOSX)
            ret = OSX.dlsym(library, function);
        else
            ret = Linux.dlsym(library, function);

        if (ret == IntPtr.Zero)
        {
            if (throwIfNotFound)
                throw new EntryPointNotFoundException(function);

#pragma warning disable CS8603 // Possible null reference return.
            return default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        return (T)(object)Marshal.GetDelegateForFunctionPointer(ret, typeof(T));
    }
}
