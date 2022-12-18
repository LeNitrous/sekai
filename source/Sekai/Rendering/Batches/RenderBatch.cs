// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public abstract class RenderBatch<T> : FrameworkObject, IRenderBatch<T>
    where T : unmanaged, IVertex
{
    public bool HasStarted { get; private set; }
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
    private readonly GraphicsContext context = Services.Current.Resolve<GraphicsContext>();

    protected RenderBatch(int maxPrimitiveCount)
    {
        maxIndexCount = maxPrimitiveCount * PrimitiveIndexCount;
        maxVertexCount = maxPrimitiveCount * PrimitiveVertexCount;

        Span<ushort> indices = stackalloc ushort[maxIndexCount];
        CreateIndices(indices);

        vertices = new T[maxVertexCount];

        shd = CreateShader();
        vbo = new(vertices.Length, true);
        ibo = new(indices.Length);
        ibo.SetData(indices);
    }

    public void Begin()
    {
        if (HasStarted)
            throw new InvalidOperationException(@"Batch has already begun.");

        shd.Bind();
        vbo.Bind();
        ibo.Bind();

        HasStarted = true;
    }

    public void End()
    {
        if (!HasStarted)
            throw new InvalidOperationException(@"Batch has not yet started.");

        Flush();

        shd.Unbind();

        HasStarted = false;
    }

    public void Add(T vertex)
    {
        ensureBound();

        if (currentVertexCount + 1 > maxVertexCount)
            Flush();

        vertices[currentVertexCount] = vertex;

        currentVertexCount++;
    }

    public void Flush()
    {
        ensureBound();

        if (currentVertexCount < PrimitiveVertexCount)
            return;

        vbo.SetData(vertices);
        context.Draw(maxIndexCount, Topology);

        currentVertexCount = 0;
    }

    private void ensureBound()
    {
        if (!HasStarted)
            throw new InvalidOperationException($"Batch must call {nameof(Begin)} before collectiong primitives.");
    }

    protected abstract Shader CreateShader();

    protected abstract void CreateIndices(Span<ushort> buffer);
}
