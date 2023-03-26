// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sekai.Graphics;

public partial class Buffer
{
    [ExcludeFromCodeCoverage]
    public static class Vertex
    {
        /// <summary>
        /// Creates a new strongly-typed vertex buffer.
        /// </summary>
        /// <typeparam name="T">The blittable vertex layout type.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <param name="count">The number of elements the buffer will contain.</param>
        /// <param name="dynamic">Whether this buffer will constantly be updated with new data.</param>
        /// <returns>A new strongly-typed vertex buffer.</returns>
        /// <exception cref="ArgumentException">Thrown when invalid arguments were passed.</exception>
        public static Buffer<T> Create<T>(GraphicsDevice device, int count, bool dynamic = false)
            where T : unmanaged, ILayout
        {
            if (count <= 0)
            {
                throw new ArgumentException("Capacity cannot be less than or equal to zero.", nameof(count));
            }

            var usage = Veldrid.BufferUsage.VertexBuffer;

            if (dynamic)
            {
                usage |= Veldrid.BufferUsage.Dynamic;
            }

            return new(device.Factory.CreateBuffer(new((uint)(count * Unsafe.SizeOf<T>()), usage)), BufferKind.Vertex, dynamic);
        }

        /// <summary>
        /// Creates a new strongly-typed vertex buffer.
        /// </summary>
        /// <typeparam name="T">The blittable vertex layout type.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <param name="data">The data the buffer will initially contain.</param>
        /// <param name="dynamic">Whether this buffer will constantly be updated with new data.</param>
        /// <returns>A new strongly-typed vertex buffer.</returns>
        public static Buffer<T> Create<T>(GraphicsDevice device, ReadOnlySpan<T> data, bool dynamic = false)
            where T : unmanaged, ILayout
        {
            var buffer = Create<T>(device, data.Length, dynamic);
            buffer.SetData(device, data);
            return buffer;
        }

        /// <summary>
        /// Creates a new strongly-typed vertex buffer.
        /// </summary>
        /// <typeparam name="T">The blittable vertex layout type.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <param name="data">The data the buffer will initially contain.</param>
        /// <param name="dynamic">Whether this buffer will constantly be updated with new data.</param>
        /// <returns>A new strongly-typed vertex buffer.</returns>
        public static Buffer<T> Create<T>(GraphicsDevice device, T[] data, bool dynamic = false)
            where T : unmanaged, ILayout
        {
            return Create(device, (ReadOnlySpan<T>)data.AsSpan(), dynamic);
        }

        /// <summary>
        /// Creates a new strongly-typed vertex buffer.
        /// </summary>
        /// <typeparam name="T">The blittable vertex layout type.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <param name="data">The data the buffer will initially contain.</param>
        /// <param name="length">The number of elements to store.</param>
        /// <param name="dynamic">Whether this buffer will constantly be updated with new data.</param>
        /// <returns>A new strongly-typed vertex buffer.</returns>
        public static Buffer<T> Create<T>(GraphicsDevice device, ref T data, int length = 1, bool dynamic = false)
            where T : unmanaged, ILayout
        {
            return Create(device, MemoryMarshal.CreateReadOnlySpan(ref data, length), dynamic);
        }

        /// <summary>
        /// Creates a new strongly-typed vertex buffer.
        /// </summary>
        /// <typeparam name="T">The blittable vertex layout type.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <param name="data">The data the buffer will initially contain.</param>
        /// <param name="dynamic">Whether this buffer will constantly be updated with new data.</param>
        /// <returns>A new strongly-typed vertex buffer.</returns>
        public static Buffer<T> Create<T>(GraphicsDevice device, T data, bool dynamic = false)
            where T : unmanaged, ILayout
        {
            return Create(device, ref data, 1, dynamic);
        }
    }
}
