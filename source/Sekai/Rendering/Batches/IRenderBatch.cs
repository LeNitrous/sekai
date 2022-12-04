// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

/// <summary>
/// Allows batching renderables to effectively reduce draw calls.
/// </summary>
public interface IRenderBatch : IDisposable
{
    /// <summary>
    /// Starts the batcher.
    /// </summary>
    void Begin();

    /// <summary>
    /// Ends the batcher flushing its current batch.
    /// </summary>
    void End();
}

/// <inheritdoc cref="IRenderBatch"/>
public interface IRenderBatch<T, U> : IRenderBatch
    where T : unmanaged, IPrimitive<U>
    where U : struct, IEquatable<U>
{
    /// <summary>
    /// Collects a primitive.
    /// </summary>
    void Collect(T primitive);
}
