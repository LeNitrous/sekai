// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

/// <inheritdoc cref="IRenderBatch"/>
public interface IRenderBatch3D<T> : IRenderBatch<T, Vector3>
    where T : unmanaged, IPrimitive<Vector3>
{
}
