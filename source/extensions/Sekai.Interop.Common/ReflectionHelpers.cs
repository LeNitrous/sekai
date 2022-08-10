// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Contains code from MonoGame.
// Copyright 2021 the MonoGame Team.
// Licensed under the Microsoft Public License and MIT.
// See https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt.
using System;
using System.Runtime.InteropServices;

namespace Sekai.Interop.Common;
internal static partial class ReflectionHelpers
{
    /// <summary>
    /// Generics handler for Marshal.SizeOf
    /// </summary>
    internal static class SizeOf<T>
    {
        private static readonly int sizeOf;

        static SizeOf()
        {
            sizeOf = Marshal.SizeOf<T>();
        }

        public static int Get()
        {
            return sizeOf;
        }
    }

    /// <summary>
    /// Fallback handler for Marshal.SizeOf(type)
    /// </summary>
    internal static int ManagedSizeOf(Type type)
    {
        return Marshal.SizeOf(type);
    }
}
