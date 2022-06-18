// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Extensions;

/// <summary>
///     A collection of extension methods for working with weak references
///     (a weak reference does not prevent an object from being garbage collected).
/// </summary>
public static class WeakReferenceExtensions
{
    /// <summary>
    ///     Gets the value of a weak reference (what the reference it to)
    /// </summary>
    /// <typeparam name="T">The type of object the weak reference holds</typeparam>
    /// <param name="wr">The weak reference</param>
    /// <returns>The value or null</returns>
    public static T? GetValue<T>(this WeakReference<T> wr)
        where T : class
    {
        if (wr.TryGetTarget(out var val))
            return val;

        return null;
    }

    /// <summary>
    ///     Adds an object to a collection of weak references
    ///     (will not add if object is null or not of right type)
    /// </summary>
    /// <typeparam name="T">The type of object ot add</typeparam>
    /// <param name="ls">The weak reference list</param>
    /// <param name="val">The object o add</param>
    public static void Add<T>(this ICollection<WeakReference<T>> ls, object val)
        where T : class
    {
        if (val is T v)
            ls.Add(new WeakReference<T>(v));
    }
}
