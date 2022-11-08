// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Sekai.Experimental.Mathematics;

/// <summary>
/// Represents an axis-aligned bounding box in three dimensional space that store only the Center and Extent.
/// </summary>
[DataContract]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BoundingBoxExt : IEquatable<BoundingBoxExt>
{
    /// <summary>
    /// A <see cref="BoundingBoxExt"/> which represents an empty space.
    /// </summary>
    public static readonly BoundingBoxExt Empty = new(BoundingBox.Empty);

    /// <summary>
    /// The center of this bounding box.
    /// </summary>
    public Vector3 Center;

    /// <summary>
    /// The extent of this bounding box.
    /// </summary>
    public Vector3 Extent;

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.BoundingBoxExt" /> struct.
    /// </summary>
    /// <param name="box">The box.</param>
    public BoundingBoxExt(BoundingBox box)
    {
        this.Center = box.Center;
        this.Extent = box.Extent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.BoundingBoxExt"/> struct.
    /// </summary>
    /// <param name="minimum">The minimum vertex of the bounding box.</param>
    /// <param name="maximum">The maximum vertex of the bounding box.</param>
    public BoundingBoxExt(Vector3 minimum, Vector3 maximum)
    {
        this.Center = (minimum + maximum) / 2;
        this.Extent = (maximum - minimum) / 2;
    }

    /// <summary>
    /// Gets the minimum.
    /// </summary>
    /// <value>The minimum.</value>
    public Vector3 Minimum => Center - Extent;

    /// <summary>
    /// Gets the maximum.
    /// </summary>
    /// <value>The maximum.</value>
    public Vector3 Maximum => Center + Extent;

    /// <summary>
    /// Transform this Bounding box
    /// </summary>
    /// <param name="world">The transform to apply to the bounding box.</param>
    public void Transform(Matrix world)
    {
        // http://zeuxcg.org/2010/10/17/aabb-from-obb-with-component-wise-abs/
        // Compute transformed AABB (by world)
        var center = Center;
        var extent = Extent;

        Vector3.TransformCoordinate(ref center, ref world, out Center);

        // Update world matrix into absolute form
        unsafe
        {
            // Perform an abs on the matrix
            float* matrixData = (float*)&world;
            for (int j = 0; j < 16; ++j)
            {
                //*matrixData &= 0x7FFFFFFF;
                *matrixData = MathF.Abs(*matrixData);
                ++matrixData;
            }
        }

        Vector3.TransformNormal(ref extent, ref world, out Extent);
    }

    /// <summary>
    /// Constructs a <see cref="Stride.Core.Mathematics.BoundingBoxExt"/> that is as large as the total combined area of the two specified boxes.
    /// </summary>
    /// <param name="value1">The first box to merge.</param>
    /// <param name="value2">The second box to merge.</param>
    /// <param name="result">When the method completes, contains the newly constructed bounding box.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Merge(ref BoundingBoxExt value1, ref BoundingBoxExt value2, out BoundingBoxExt result)
    {
        var maximum = Vector3.Max(value1.Maximum, value2.Maximum);
        var minimum = Vector3.Min(value1.Minimum, value2.Minimum);

        result.Center = (minimum + maximum) / 2;
        result.Extent = (maximum - minimum) / 2;
    }

    /// <inheritdoc/>
    public bool Equals(BoundingBoxExt other)
    {
        return Center.Equals(other.Center) && Extent.Equals(other.Extent);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        return obj is BoundingBoxExt ext && Equals(ext);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            return (Center.GetHashCode() * 397) ^ Extent.GetHashCode();
        }
    }

    /// <summary>
    /// Implements the ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(BoundingBoxExt left, BoundingBoxExt right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Implements the !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(BoundingBoxExt left, BoundingBoxExt right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="BoundingBoxExt"/> to <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="bbExt">The bb ext.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator BoundingBox(BoundingBoxExt bbExt)
    {
        return new BoundingBox(bbExt.Minimum, bbExt.Maximum);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="BoundingBox"/> to <see cref="BoundingBoxExt"/>.
    /// </summary>
    /// <param name="boundingBox">The bounding box.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator BoundingBoxExt(BoundingBox boundingBox)
    {
        return new BoundingBoxExt(boundingBox);
    }
}
