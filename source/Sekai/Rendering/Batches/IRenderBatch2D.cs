// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

/// <inheritdoc cref="IRenderBatch"/>
public interface IRenderBatch2D<T> : IRenderBatch<T, Vector2>
    where T : unmanaged, IPrimitive<Vector2>
{
}
