// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Buffers;

namespace Sekai.OpenGL;

internal class GLBuffer : NativeBuffer
{
    private readonly uint bufferId;
    private readonly GLGraphicsSystem system;

    public GLBuffer(GLGraphicsSystem system, uint bufferId, int capacity, bool dynamic)
        : base(capacity, dynamic)
    {
        this.system = system;
        this.bufferId = bufferId;
    }

    public override void GetData(nint dest, int size, int offset = 0) => system.GetBufferData(bufferId, dest, size, offset);

    public override void SetData(nint data, int size, int offset = 0) => system.SetBufferData(bufferId, data, size, offset);

    protected override void Destroy() => system.DestroyBuffer(bufferId);

    public static implicit operator uint(GLBuffer buffer) => buffer.bufferId;
}
