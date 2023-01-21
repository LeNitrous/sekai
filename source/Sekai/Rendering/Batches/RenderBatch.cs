// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public abstract class RenderBatch<T> : DependencyObject, IRenderBatch<T>
    where T : unmanaged, IVertex
{
    public bool HasStarted { get; private set; }
    protected abstract string Shader { get; }
    protected abstract int PrimitiveIndexCount { get; }
    protected abstract int PrimitiveVertexCount { get; }
    protected abstract PrimitiveTopology Topology { get; }

    private int currentVertexCount;
    private readonly int maxIndexCount;
    private readonly int maxVertexCount;
    private readonly T[] vertices;
    private readonly Shader shd;
    private readonly Graphics.Buffers.Buffer<T> vbo;
    private readonly Graphics.Buffers.Buffer<ushort> ibo;

    [Resolved]
    protected GraphicsContext Graphics { get; set; } = null!;

    [Resolved]
    private AssetLoader assets { get; set; } = null!;

    protected RenderBatch(int maxPrimitiveCount)
    {
        maxIndexCount = maxPrimitiveCount * PrimitiveIndexCount;
        maxVertexCount = maxPrimitiveCount * PrimitiveVertexCount;

        Span<ushort> indices = stackalloc ushort[maxIndexCount];
        CreateIndices(indices);

        vertices = new T[maxVertexCount];

        shd = assets.Load<Shader>(Shader);
        vbo = new(vertices.Length, true);
        ibo = new(indices.Length);
        ibo.SetData(indices);
    }

    public void Begin()
    {
        if (HasStarted)
            throw new InvalidOperationException(@"Batch has already begun.");

        Graphics.Capture();
        Graphics.SetShader(shd);
        Graphics.SetBuffer(ibo);
        Graphics.SetBuffer(vbo);

        Prepare();

        HasStarted = true;
    }

    public void End()
    {
        if (!HasStarted)
            throw new InvalidOperationException(@"Batch has not yet started.");

        Flush();

        Graphics.Restore();

        Cleanup();

        HasStarted = false;
    }

    public void Add(T vertex)
    {
        ensureBegun();

        if (currentVertexCount + 1 > maxVertexCount)
            Flush();

        vertices[currentVertexCount] = vertex;

        currentVertexCount++;
    }

    public void Flush()
    {
        ensureBegun();

        if (currentVertexCount < PrimitiveVertexCount)
            return;

        vbo.SetData(vertices);
        Graphics.DrawIndexed((uint)maxIndexCount, Topology);

        currentVertexCount = 0;
    }

    private void ensureBegun()
    {
        if (!HasStarted)
            throw new InvalidOperationException($"Batch must call {nameof(Begin)} before collectiong primitives.");
    }

    protected virtual void Prepare()
    {
    }

    protected virtual void Cleanup()
    {
    }

    protected abstract void CreateIndices(Span<ushort> buffer);

    protected override void Destroy()
    {
        shd.Dispose();
        ibo.Dispose();
        vbo.Dispose();
    }
}
