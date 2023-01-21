// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Buffers;

/// <summary>
/// A buffer graphics device.
/// </summary>
public abstract class NativeBuffer : FrameworkObject
{
    /// <summary>
    /// The number in bytes of how much this buffer can store.
    /// </summary>
    public readonly int Capacity;

    /// <summary>
    /// Whether this buffer is dynamic or not.
    /// </summary>
    public readonly bool Dynamic;

    protected NativeBuffer(int capacity, bool dynamic = false)
    {
        Dynamic = dynamic;
        Capacity = capacity;
    }

    /// <summary>
    /// Sets the data in the buffer.
    /// </summary>
    /// <param name="data">The pointer to the data to store.</param>
    /// <param name="size">The size of the data being stored.</param>
    /// <param name="offset">The offset in the buffer to store the data.</param>
    public abstract void SetData(nint data, int size, int offset = 0);

    /// <summary>
    /// Gets the data in the buffer.
    /// </summary>
    /// <param name="dest">The pointer to the destination where data will be copied to.</param>
    /// <param name="size">The number of bytes to copy.</param>
    /// <param name="offset">The offset in the buffer to retrieve the data.</param>
    public abstract void GetData(nint dest, int size, int offset = 0);
}
