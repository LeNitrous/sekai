// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;

namespace Sekai.Graphics;

/// <summary>
/// Provides a structured wrapper for a <see cref="GraphicsBuffer"/>'s contents.
/// </summary>
/// <typeparam name="T">The type to represent the data as.</typeparam>
public readonly struct GraphicsBufferView<T> : IDisposable
    where T : unmanaged
{
    /// <summary>
    /// Gets or sets the value at a given index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The value at the given index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.</exception>
    public unsafe T this[int index]
    {
        readonly get
        {
            if (index >= Length || index < 0)
            {
                throw new IndexOutOfRangeException($"Index must be non-negative or less than the length.");
            }

            T value = default;
            Unsafe.Copy(ref value, (byte*)data + (index * Unsafe.SizeOf<T>()));

            return value;
        }
        set
        {
            if (index >= Length || index < 0)
            {
                throw new IndexOutOfRangeException($"Index must be non-negative or less than the length.");
            }

            Unsafe.Copy((byte*)data + (index * Unsafe.SizeOf<T>()), ref value);
        }
    }

    public int Length => (int)size / Unsafe.SizeOf<T>();

    private readonly uint size;
    private readonly nint data;
    private readonly Action dispose;

    internal GraphicsBufferView(uint size, nint data, Action dispose)
    {
        this.size = size;
        this.data = data;
        this.dispose = dispose;
    }

    public readonly void Dispose() => dispose.Invoke();
}
