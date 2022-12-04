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
public abstract class RenderBatch<TPrimitive, TVertex, TVector> : FrameworkObject, IRenderBatch<TPrimitive, TVector>
    where TPrimitive : unmanaged, IPrimitive<TVector>
    where TVertex : unmanaged, IVertex
    where TVector : struct, IEquatable<TVector>
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
    private readonly TVertex[] vArray;
    private readonly Graphics.Buffers.Buffer<ushort> iBuffer;
    private readonly Graphics.Buffers.Buffer<TVertex> vBuffer;

    public RenderBatch(int maxPrimitiveCount)
    {
        this.maxPrimitiveCount = maxPrimitiveCount;

        shader = CreateShader();

        var indices = CreateIndices(maxIndexCount);

        if (indices.Length != maxIndexCount)
            throw new InvalidOperationException($"Unexpected index count. Expected {maxIndexCount}, got {indices.Length}.");

        iBuffer = new(maxIndexCount);
        iBuffer.SetData(indices);

        vArray = new TVertex[maxVertexCount];
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

        var vertices = CreateVertices(primitive.GetVertices());

        if (vertices.Length != PrimitiveVertexCount)
            throw new InvalidOperationException($"Unexpected vertex count. Expected {PrimitiveVertexCount}, got {vertices.Length}.");

        if (currentVertexCount + vertices.Length > maxVertexCount)
            Flush();

        for (int i = 0; i < PrimitiveVertexCount; i++)
            vArray[currentVertexCount + i] = vertices[i];

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

        vBuffer.SetData(vArray);
        Context.Draw(maxIndexCount, Topology);
        vArray.AsSpan().Clear();
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
    protected abstract ReadOnlySpan<TVertex> CreateVertices(ReadOnlySpan<TVector> vector);

    /// <summary>
    /// Creates a span of indices used by the index buffer.
    /// </summary>
    protected abstract ReadOnlySpan<ushort> CreateIndices(int maxVertexCount);
}
