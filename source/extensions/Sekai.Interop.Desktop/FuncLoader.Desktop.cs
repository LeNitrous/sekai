// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Contains code from MonoGame.
// Copyright 2021 the MonoGame Team.
// Licensed under the Microsoft Public License and MIT.
// See https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt.
using System.Runtime.InteropServices;

namespace Sekai.Interop.Desktop;
public class FuncLoader
{
    public static IntPtr LoadLibraryExt(string libname)
    {
        var ret = IntPtr.Zero;
        string? assemblyLocation = Path.GetDirectoryName(typeof(FuncLoader).Assembly.Location) ?? Environment.CurrentDirectory;

        // Try .NET Framework / mono locations
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ret = NativeLibrary.Load(Path.Combine(assemblyLocation, libname));

            // Look in Frameworks for .app bundles
            if (ret == IntPtr.Zero)
                ret = NativeLibrary.Load(Path.Combine(assemblyLocation, "..", "Frameworks", libname));
        }
        else
        {
            if (Environment.Is64BitProcess)
                ret = NativeLibrary.Load(Path.Combine(assemblyLocation, "x64", libname));
            else
                ret = NativeLibrary.Load(Path.Combine(assemblyLocation, "x86", libname));
        }

        // Try .NET Core development locations
        if (ret == IntPtr.Zero)
            ret = NativeLibrary.Load(Path.Combine(assemblyLocation, "runtimes", RuntimeInformation.RuntimeIdentifier, "native", libname));

        // Try current folder (.NET Core will copy it there after publish)
        if (ret == IntPtr.Zero)
            ret = NativeLibrary.Load(Path.Combine(assemblyLocation, libname));

        // Try alternate way of checking current folder
        // assemblyLocation is null if we are inside macOS app bundle
        if (ret == IntPtr.Zero)
            ret = NativeLibrary.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, libname));

        // Try loading system library
        if (ret == IntPtr.Zero)
            ret = NativeLibrary.Load(libname);

        // Welp, all failed, PANIC!!!
        if (ret == IntPtr.Zero)
            throw new Exception("Failed to load library: " + libname);

        return ret;
    }

    public static T LoadFunction<T>(IntPtr handle, string function)
    {
        try
        {
            // let's discern symbols from a generic, then return a Delegate out of that.
            return (T)(object)Marshal.GetDelegateForFunctionPointer(NativeLibrary.GetExport(handle, function), typeof(T));

        }
        catch (EntryPointNotFoundException e)
        {
            throw new EntryPointNotFoundException("Could not find function, check if the defined symbol is exported in the library.", e);
        }
    }
}
