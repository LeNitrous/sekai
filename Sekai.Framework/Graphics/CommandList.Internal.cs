// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Veldrid;

namespace Sekai.Framework.Graphics;

public partial class CommandList
{
    private bool hasStarted;
    private readonly Dictionary<GraphicsPipelineDescription, Pipeline> graphicsPipelineCache = new();

    internal override Veldrid.CommandList Resource { get; }

    public CommandList()
    {
        Resource = Context.Resources.CreateCommandList();
    }

    internal void Start()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(CommandList));

        if (hasStarted)
            throw new InvalidOperationException(@"This command list has already started.");

        hasStarted = true;
        Resource.Begin();
    }

    internal void End()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(CommandList));

        if (!hasStarted)
            throw new InvalidOperationException($"{nameof(Start)} must be called before {nameof(End)} can be called.");

        hasStarted = false;
        Resource.End();
        Context.Device.SubmitCommands(Resource);
    }

    internal void CopyTexture(Texture src, Texture dst)
    {
        ensureValidState();
        Resource.CopyTexture(src, dst);
    }

    internal void CopyTexture(Texture src, Texture dst, int mipLevel, int layer)
    {
        ensureValidState();
        Resource.CopyTexture(src, dst, (uint)mipLevel, (uint)layer);
    }

    internal void CopyTexture(Texture src, int srcX, int srcY, int srcZ, int srcMipLevel, int srcBaseArrayLayer, Texture dst, int dstX, int dstY, int dstZ, int dstMipLevel, int dstBaseArrayLayer, int width, int height, int depth, int layers)
    {
        ensureValidState();
        Resource.CopyTexture(src, (uint)srcX, (uint)srcY, (uint)srcZ, (uint)srcMipLevel, (uint)srcBaseArrayLayer, dst, (uint)dstX, (uint)dstY, (uint)dstZ, (uint)dstMipLevel, (uint)dstBaseArrayLayer, (uint)width, (uint)height, (uint)depth, (uint)layers);
    }

    internal void CopyBuffer(DeviceBuffer source, DeviceBuffer destination, int size, int sourceOffset, int destinationOffset)
    {
        ensureValidState();
        Resource.CopyBuffer(source, (uint)sourceOffset, destination, (uint)destinationOffset, (uint)size);
    }

    internal void UpdateBuffer(DeviceBuffer buffer, int offset, IntPtr source)
    {
        ensureValidState();
        Resource.UpdateBuffer(buffer, (uint)offset, source);
    }

    internal void UpdateBuffer<T>(DeviceBuffer buffer, int offset, ref T source)
        where T : unmanaged
    {
        ensureValidState();
        Resource.UpdateBuffer(buffer, (uint)offset, source);
    }

    internal void UpdateBuffer<T>(DeviceBuffer buffer, int offset, T[] source)
        where T : unmanaged
    {
        ensureValidState();
        Resource.UpdateBuffer(buffer, (uint)offset, source);
    }

    private Pipeline fetchPipeline(GraphicsPipelineDescription description)
    {
        if (!graphicsPipelineCache.TryGetValue(description, out var pipeline))
        {
            graphicsPipelineCache[description] = pipeline = Context.Resources.CreateGraphicsPipeline(description);
        }

        return pipeline;
    }

    private void ensureValidState()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(CommandList));

        if (!hasStarted)
            throw new InvalidOperationException($"{nameof(Start)} must be called before this operation can be started.");
    }

    private static readonly Dictionary<Type, IndexFormat> index_format_map = new()
    {
        { typeof(int), IndexFormat.UInt32 },
        { typeof(uint), IndexFormat.UInt32 },
        { typeof(short), IndexFormat.UInt16 },
        { typeof(ushort), IndexFormat.UInt16 },
    };
}
