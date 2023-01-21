// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

/// <summary>
/// An object that batches render operations.
/// </summary>
public interface IRenderBatch : IDisposable
{
    /// <summary>
    /// Begins a batch.
    /// </summary>
    void Begin();

    /// <summary>
    /// Ends the current batch.
    /// </summary>
    void End();

    /// <summary>
    /// Flushes the current batch.
    /// </summary>
    void Flush();
}

/// <summary>
/// An object that batches render operations.
/// </summary>
/// <typeparam name="T">The vertex type to accept.</typeparam>
public interface IRenderBatch<T> : IRenderBatch
    where T : unmanaged, IVertex
{
    /// <summary>
    /// Adds a vertex to the current batch.
    /// </summary>
    /// <param name="vertex">The vertex to add.</param>
    void Add(T vertex);
}
