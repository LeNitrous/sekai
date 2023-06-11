// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Buffers;
using System.Drawing;
using System.IO;

namespace Sekai.Platform;

/// <summary>
/// Represents an icon used by <see cref="IWindow"/>.
/// </summary>
public class Icon : IDisposable, IEquatable<Icon>
{
    /// <summary>
    /// An empty icon.
    /// </summary>
    public static readonly Icon Empty = new();

    /// <summary>
    /// The icon's byte data.
    /// </summary>
    public Span<byte> Data => data is not null ? data.Memory.Span : Span<byte>.Empty;

    /// <summary>
    /// The icon's size.
    /// </summary>
    public Size Size { get; }

    private bool isDisposed;
    private readonly IMemoryOwner<byte>? data;

    /// <summary>
    /// Create an icon from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to copy contents from.</param>
    public Icon(Size size, Stream stream)
    {
        Size = size;
        data = MemoryPool<byte>.Shared.Rent((int)stream.Length);
        stream.Read(data.Memory.Span);
        stream.Position = 0;
    }

    /// <summary>
    /// Creates an icon from a <see langword="byte[]"/>
    /// </summary>
    /// <param name="bytes">The byte array to copy contents from.</param>
    public Icon(Size size, byte[] bytes)
    {
        Size = size;
        data = MemoryPool<byte>.Shared.Rent(bytes.Length);
        bytes.CopyTo(data.Memory);
    }

    /// <summary>
    /// Creates an empty icon.
    /// </summary>
    private Icon()
    {
    }

    ~Icon()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        data?.Dispose();

        isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public bool Equals(Icon? other)
    {
        return other is not null && data is not null && other.data is not null && data.Memory.Span.SequenceEqual(other.data.Memory.Span);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Icon);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(isDisposed, data);
    }
}
