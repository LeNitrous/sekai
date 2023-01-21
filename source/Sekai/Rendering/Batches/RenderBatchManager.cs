// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System;
using Sekai.Graphics.Vertices;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

internal class RenderBatchManager : FrameworkObject
{
    private IRenderBatch? currentBatch;
    private const int max_primitives_per_batch = 1000;
    private readonly Dictionary<Type, IRenderBatch> batches = new();

    public RenderBatchManager()
    {
        addBatch<Quad, TexturedVertex2D>(new QuadBatch(max_primitives_per_batch));
        addBatch<Line2D, ColoredVertex2D>(new LineBatch2D(max_primitives_per_batch));
        addBatch<Line3D, ColoredVertex3D>(new LineBatch3D(max_primitives_per_batch));
    }

    /// <summary>
    /// Gets a render batch from this manager.
    /// </summary>
    /// <typeparam name="T">The primitive type this render batch is assigned to.</typeparam>
    /// <typeparam name="U">The vertex type the render batch uses.</typeparam>
    /// <param name="batch">The returned vertex.</param>
    /// <returns>True if the current active batch changed. False otherwise.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if there is no render batch assigned to the given type.</exception>
    /// <exception cref="InvalidCastException">Thrown if the found render batch cannot be casted to a typed render batch.</exception>
    public bool GetBatch<T, U>(out IRenderBatch<U> batch)
        where T : unmanaged, IEquatable<T>
        where U : unmanaged, IVertex
    {
        if (!batches.TryGetValue(typeof(T), out var b))
            throw new KeyNotFoundException($"{typeof(T)} has no associated render batch.");

        if (b is not IRenderBatch<U> typedBatch)
            throw new InvalidCastException($"Cannot cast render batch.");

        bool changed = false;

        if (currentBatch != b)
        {
            currentBatch?.End();
            currentBatch = b;
            currentBatch.Begin();
            changed = true;
        }

        batch = typedBatch;
        return changed;
    }

    public void EndCurrentBatch()
    {
        currentBatch?.End();
        currentBatch = null;
    }

    private void addBatch<T, U>(IRenderBatch<U> batch)
        where T : unmanaged, IEquatable<T>
        where U : unmanaged, IVertex
    {
        if (batches.ContainsKey(typeof(T)))
            throw new ArgumentException($"A render batch for {typeof(T)} is already registered.", nameof(T));

        batches.Add(typeof(T), batch);
    }

    protected override void Destroy()
    {
        foreach (var batch in batches.Values)
            batch.Dispose();
    }
}
