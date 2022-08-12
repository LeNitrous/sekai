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
    /// <summary>
    /// Loads the library from a given name.
    /// Depending on the runtime, it will try to load from the current working directory, then to the system.
    /// On macOS, it will try to load from the the .NET Framework/Mono locations first then the system.
    /// </summary>
    /// <param name="name">The name of the library to load.</param>
    /// <returns>The handle to the library.</returns>
    public static IntPtr LoadLibraryExt(string libName)
    {
        var ret = IntPtr.Zero;
        string? assemblyLocation = Path.GetDirectoryName(typeof(FuncLoader).Assembly.Location) ?? Environment.CurrentDirectory;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // let's check if we can load it from the .NET Framework or the Mono Locations
            if (NativeLibrary.TryLoad(Path.Combine(assemblyLocation, libName), out var handle))
              ret = handle;
            // no? let's try the Framework .app bundle locations.
            else if (NativeLibrary.TryLoad(Path.Combine(assemblyLocation, "..", "Frameworks", libName), out var syshandle))
              ret = syshandle;
            else
              ret = IntPtr.Zero;
        }
        else
        {
            if (Environment.Is64BitProcess)
               ret = NativeLibrary.Load(Path.Combine(assemblyLocation, "x64", libName));
            else
              ret = NativeLibrary.Load(Path.Combine(assemblyLocation, "x86", libName));
        }

        // Try .NET Core development locations
        if (ret == IntPtr.Zero)
        {
          if (NativeLibrary.TryLoad(Path.Combine(assemblyLocation, "runtimes", RuntimeInformation.RuntimeIdentifier, "native", libName), out var handle))
            ret = handle;
          else
            ret = IntPtr.Zero;
        }

        // Try current folder (usually this is the production build from `dotnet publish` goes to.)
        if (ret == IntPtr.Zero)
        {
          if (NativeLibrary.TryLoad(Path.Combine(Environment.CurrentDirectory, libName), out var handle))
            ret = handle;
          else
            ret = IntPtr.Zero;
        }

        // last-ditch effort: load from the system library and pray it works
        if (ret == IntPtr.Zero)
        {
          if (NativeLibrary.TryLoad(libName, out var handle))
            ret = handle;
          else
            ret = IntPtr.Zero;
        }

        // nothing works? too bad, let's tell everyone we can't load it.
        if (ret == IntPtr.Zero)
          throw new Exception($"Could not load library {libName}");


        return ret;
    }

    /// <summary>
    /// Loads the function provided the handle to the library is supplied.
    /// </summary>
    /// <param name="handle">The handle to the library.</param>
    /// <param name="function">The name of the function to load.</param>
    /// <returns>The function pointer.</returns>
    public static T LoadFunction<T>(IntPtr handle, string function)
    {
        var ret = IntPtr.Zero;

        if (NativeLibrary.TryGetExport(handle, function, out var funcHandle))
        {
            ret = funcHandle;
        }
        else
        {
            throw new Exception($"Could not find function {function}");
        }

        return (T)(object)Marshal.GetDelegateForFunctionPointer(ret, typeof(T));
    }
}
