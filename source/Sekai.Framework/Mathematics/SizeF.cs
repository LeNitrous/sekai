// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Runtime.InteropServices;

namespace Sekai.Framework.Mathematics;

/// <summary>
/// Defines a 2D rectangular size (width,height).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SizeF : IEquatable<SizeF>
{
    /// <summary>
    /// A zero size with (width, height) = (0,0)
    /// </summary>
    public static readonly SizeF Zero = new(0, 0);

    /// <summary>
    /// A zero size with (width, height) = (0,0)
    /// </summary>
    public static readonly SizeF Empty = Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeF"/> struct.
    /// </summary>
    /// <param name="width">The x.</param>
    /// <param name="height">The y.</param>
    public SizeF(float width, float height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Width.
    /// </summary>
    public float Width;

    /// <summary>
    /// Height.
    /// </summary>
    public float Height;

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Equals(SizeF other)
    {
        return other.Width == Width && other.Height == Height;
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        return obj is SizeF size && Equals(size);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(SizeF left, SizeF right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(SizeF left, SizeF right)
    {
        return !left.Equals(right);
    }

    public static explicit operator Size(SizeF value)
    {
        return new Size((int)value.Width, (int)value.Height);
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return string.Format("({0},{1})", Width, Height);
    }
}
