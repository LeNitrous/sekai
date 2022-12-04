// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A geometric primitive.
/// </summary>
public interface IPrimitive
{
}

/// <summary>
/// A strongly typed geometric primitive.
/// </summary>
public interface IPrimitive<T> : IPrimitive
    where T : unmanaged, IEquatable<T>
{
    /// <summary>
    /// Retrieves the points of this primitive.
    /// </summary>
    /// <returns>The polygon's points.</returns>
    ReadOnlySpan<T> GetPoints();
}
