// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.Serialization;

namespace Sekai.Framework.Structures;

/// <summary>
/// A reimplementation of the .NET Framework WeakReference<T>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WeakReference<T> : WeakReference, ISerializable
    where T : class
{
    #region Constructors inherited from WeakReference
    /// <summary>
    /// Creates a new WeakReference that keeps track of a target.
    /// Assumes a Short Weak Reference (ie, track resurrection is false)
    /// </summary>
    public WeakReference(T target) : base(target) { }

    public WeakReference(object? target) : base(target) { }

    /// <summary>
    /// Creates a new WeakReference that keeps track of a target.
    /// </summary>
    public WeakReference(T target, bool trackResurrection) : base(target, trackResurrection) { }

    protected WeakReference(SerializationInfo info, StreamingContext context) : base(info, context) { }
    #endregion

    public void SetTarget(T target)
    {
        Target = target;
    }

    // NOTE: we do not implement GetObjectData(), this is already implemented at WeakReference.
    // However the following method above was present in .NET Framework but doesn't exist anymore .
    // so this will be implemented.
    public bool TryGetTarget(out T target)
    {
        var o = (T?)Target;
        target = o!;
        return o != null;
    }
}

