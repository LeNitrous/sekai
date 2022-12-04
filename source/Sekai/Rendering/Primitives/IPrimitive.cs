// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A geometric primitive.
/// </summary>
public interface IPrimitive<T>
{
    /// <summary>
    /// Retrieves the vertices of this primitive.
    /// </summary>
    /// <returns>The polygon's vertices.</returns>
    ReadOnlySpan<T> GetVertices();
}
