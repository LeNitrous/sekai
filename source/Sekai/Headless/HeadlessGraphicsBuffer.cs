// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using Sekai.Graphics;

namespace Sekai.Headless;

internal sealed class HeadlessGraphicsBuffer : GraphicsBuffer
{
    public override int Capacity { get; }

    public override BufferType Type { get; }

    public override bool Dynamic { get; }

    private bool isDisposed;
    private MemoryHandle? handle;
    private readonly IMemoryOwner<byte> owner;

    public HeadlessGraphicsBuffer(int capacity, BufferType type, bool dynamic)
    {
        Capacity = capacity;
        Type = type;
        Dynamic = dynamic;
        owner = MemoryPool<byte>.Shared.Rent(capacity);
    }

    protected override unsafe nint Map(MapMode mode)
    {
        handle = owner.Memory.Pin();
        return (nint)handle.Value.Pointer;
    }

    protected override void Unmap()
    {
        handle?.Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        owner.Dispose();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    public override unsafe void SetData(nint data, uint size, uint offset = 0)
    {
        Span<byte> src = new(data.ToPointer(), (int)size);
        src.CopyTo(owner.Memory.Span[(int)offset..]);
    }

    public override unsafe void GetData(nint data, uint size, uint offset = 0)
    {
        Span<byte> dst = new(data.ToPointer(), (int)size);
        owner.Memory.Span[(int)offset..].CopyTo(dst);
    }
}
