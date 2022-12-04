// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

/// <inheritdoc cref="IRenderBatch"/>
public abstract class RenderBatch<TPrimitive, TVertex, TPoint> : FrameworkObject, IRenderBatch<TPrimitive, TPoint>
    where TPrimitive : unmanaged, IPrimitive<TPoint>
    where TVertex : unmanaged, IVertex
    where TPoint : unmanaged, IEquatable<TPoint>
{
    /// <summary>
    /// Gets whether this batch has begun and can collect primitives.
    /// </summary>
    public bool HasStarted { get; private set; }

    /// <summary>
    /// The amount of indices per primitive.
    /// </summary>
    protected abstract int PrimitiveIndexCount { get; }

    /// <summary>
    /// The amount of vertices per primitive.
    /// </summary>
    protected abstract int PrimitiveVertexCount { get; }

    /// <summary>
    /// The primitive topology used by this render batch.
    /// </summary>
    protected abstract PrimitiveTopology Topology { get; }

    /// <summary>
    /// The current graphics context.
    /// </summary>
    protected readonly GraphicsContext Context = Services.Current.Resolve<GraphicsContext>();

    private int maxIndexCount => maxPrimitiveCount * PrimitiveIndexCount;
    private int maxVertexCount => maxPrimitiveCount * PrimitiveVertexCount;

    private int currentVertexCount;
    private readonly int maxPrimitiveCount;
    private readonly Shader shader;
    private readonly TVertex[] vertices;
    private readonly Graphics.Buffers.Buffer<ushort> iBuffer;
    private readonly Graphics.Buffers.Buffer<TVertex> vBuffer;

    public RenderBatch(int maxPrimitiveCount)
    {
        this.maxPrimitiveCount = maxPrimitiveCount;

        shader = CreateShader();

        Span<ushort> indices = stackalloc ushort[maxIndexCount];
        CreateIndices(indices);

        iBuffer = new(maxIndexCount);
        iBuffer.SetData(indices);

        vertices = new TVertex[maxVertexCount];
        vBuffer = new(maxVertexCount, true);
    }

    /// <summary>
    /// Starts the batcher allowing subsequent <see cref="Collect"/> calls to be made.
    /// </summary>
    public void Begin()
    {
        if (HasStarted)
            throw new InvalidOperationException(@"Batch has already begun.");

        shader.Bind();
        iBuffer.Bind();
        vBuffer.Bind();

        OnBegin();

        HasStarted = true;
    }

    /// <summary>
    /// Ends the batcher drawing all collected primitives to the current framebuffer.
    /// </summary>
    public void End()
    {
        if (!HasStarted)
            throw new InvalidOperationException(@"Batch has not yet started.");

        Flush();

        OnEnd();
        shader.Unbind();

        HasStarted = false;
    }

    public void Collect(TPrimitive primitive)
    {
        EnsureStarted();

        if (currentVertexCount + PrimitiveVertexCount > maxVertexCount)
            Flush();

        CreateVertices(vertices.AsSpan(currentVertexCount, PrimitiveVertexCount), primitive.GetPoints());

        currentVertexCount += PrimitiveVertexCount;
    }

    /// <summary>
    /// Draws all collected primitives without ending the batcher.
    /// </summary>
    protected void Flush()
    {
        EnsureStarted();

        if (currentVertexCount == 0)
            return;

        vBuffer.SetData(vertices);
        Context.Draw(maxIndexCount, Topology);
        vertices.AsSpan().Clear();
        currentVertexCount = 0;
    }

    /// <summary>
    /// Ensures that the render batch has started.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the render batch has not yet started.</exception>
    protected void EnsureStarted()
    {
        if (!HasStarted)
            throw new InvalidOperationException($"Batch must call {nameof(Begin)} before collectiong primitives.");
    }

    /// <summary>
    /// Called when the current batch has begun.
    /// </summary>
    protected virtual void OnBegin()
    {
    }

    /// <summary>
    /// Called when the current batch has ended.
    /// </summary>
    protected virtual void OnEnd()
    {
    }

    /// <summary>
    /// Creates the shader used by this render batch.
    /// </summary>
    protected abstract Shader CreateShader();

    /// <summary>
    /// Creates a vertex for a given position vector.
    /// </summary>
    protected abstract void CreateVertices(Span<TVertex> buffer, ReadOnlySpan<TPoint> vector);

    /// <summary>
    /// Creates a span of indices used by the index buffer.
    /// </summary>
    protected abstract void CreateIndices(Span<ushort> buffer);
}
