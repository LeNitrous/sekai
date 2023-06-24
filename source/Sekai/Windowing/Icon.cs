// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using Sekai.Mathematics;

namespace Sekai.Windowing;

/// <summary>
/// Represents an icon used by <see cref="IWindow"/>.
/// </summary>
public readonly struct Icon : IEquatable<Icon>
{
    /// <summary>
    /// An empty icon.
    /// </summary>
    public static readonly Icon Empty = new();

    /// <summary>
    /// The icon's size.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// The icon's pixels.
    /// </summary>
    public ReadOnlyMemory<byte> Pixels { get; }

    /// <summary>
    /// Create an icon from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to copy contents from.</param>
    public Icon(Size size, Stream stream)
    {
        Size = size;

        using var memory = new MemoryStream();
        stream.CopyTo(memory);

        Pixels = memory.ToArray();
    }

    /// <summary>
    /// Creates an icon from a <see cref="Memory{byte}">
    /// </summary>
    /// <param name="pixels">The byte memory to get contents from.</param>
    public Icon(Size size, Memory<byte> pixels)
    {
        Size = size;
        Pixels = pixels;
    }

    public bool Equals(Icon other)
    {
        return Size.Equals(other.Size) && Pixels.Span.SequenceEqual(other.Pixels.Span);
    }

    public override bool Equals(object? obj)
    {
        return obj is Icon icon && Equals(icon);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Size, Pixels);
    }

    public static bool operator ==(Icon left, Icon right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Icon left, Icon right)
    {
        return !(left == right);
    }
}
