// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Contains code from MonoGame.
// Copyright 2021 the MonoGame Team.
// Licensed under the Microsoft Public License and MIT.
// See https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt.
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sekai.Interop.Common;
public static class InteropHelpers
{
    /// <summary>
    /// Convert a pointer to a Utf8 null-terminated string to a .NET System.String
    /// </summary>
    public static unsafe string Utf8ToString(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return string.Empty;

        byte* ptr = (byte*)handle;
        while (*ptr != 0)
            ptr++;

        long len = ptr - (byte*)handle;
        if (len == 0)
            return string.Empty;

        byte[]? bytes = new byte[len];
        Marshal.Copy(handle, bytes, 0, bytes.Length);

        return Encoding.UTF8.GetString(bytes);
    }
}
