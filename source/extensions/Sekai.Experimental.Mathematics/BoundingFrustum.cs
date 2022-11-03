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

namespace Sekai.Experimental.Mathematics;
/// <summary>
/// A bounding frustum.
/// </summary>
public struct BoundingFrustum
{
    /// <summary>
    /// The left plane of this frustum.
    /// </summary>
    public Plane LeftPlane;

    /// <summary>
    /// The right plane of this frustum.
    /// </summary>
    public Plane RightPlane;

    /// <summary>
    /// The top  plane of this frustum.
    /// </summary>
    public Plane TopPlane;

    /// <summary>
    /// The bottom plane of this frustum.
    /// </summary>
    public Plane BottomPlane;

    /// <summary>
    /// The near plane of this frustum.
    /// </summary>
    public Plane NearPlane;

    /// <summary>
    /// The far plane of this frustum.
    /// </summary>
    public Plane FarPlane;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingFrustum"/> struct from a matrix view-projection.
    /// </summary>
    /// <param name="matrix">The matrix view projection.</param>
    public BoundingFrustum(ref Matrix matrix)
    {
        // Left
        Plane.Normalize(
            matrix.M14 + matrix.M11,
            matrix.M24 + matrix.M21,
            matrix.M34 + matrix.M31,
            matrix.M44 + matrix.M41,
            out LeftPlane);

        // Right
        Plane.Normalize(
            matrix.M14 - matrix.M11,
            matrix.M24 - matrix.M21,
            matrix.M34 - matrix.M31,
            matrix.M44 - matrix.M41,
            out RightPlane);

        // Top
        Plane.Normalize(
            matrix.M14 - matrix.M12,
            matrix.M24 - matrix.M22,
            matrix.M34 - matrix.M32,
            matrix.M44 - matrix.M42,
            out TopPlane);

        // Bottom
        Plane.Normalize(
            matrix.M14 + matrix.M12,
            matrix.M24 + matrix.M22,
            matrix.M34 + matrix.M32,
            matrix.M44 + matrix.M42,
            out BottomPlane);

        // Near
        Plane.Normalize(
            matrix.M13,
            matrix.M23,
            matrix.M33,
            matrix.M43,
            out NearPlane);

        // Far
        Plane.Normalize(
            matrix.M14 - matrix.M13,
            matrix.M24 - matrix.M23,
            matrix.M34 - matrix.M33,
            matrix.M44 - matrix.M43,
            out FarPlane);
    }

    /// <summary>
    /// Check whether this frustum contains the specified <see cref="BoundingBoxExt"/>.
    /// </summary>
    /// <param name="boundingBoxExt">The bounding box.</param>
    /// <returns><c>true</c> if this frustum contains the specified bounding box.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(ref BoundingBoxExt boundingBoxExt)
    {
        return CollisionHelper.FrustumContainsBox(ref this, ref boundingBoxExt);
    }
}
