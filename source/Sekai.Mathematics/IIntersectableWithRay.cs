// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Allows to determine intersections with a <see cref="Sekai.Mathematics.Ray"/>.
/// </summary>
public interface IIntersectableWithRay
{
    /// <summary>
    /// Determines if there is an intersection between the current object and a <see cref="Sekai.Mathematics.Ray"/>.
    /// </summary>
    /// <param name="ray">The ray to test.</param>
    /// <returns>Whether the two objects intersected.</returns>
    public bool Intersects(ref Ray ray);

    /// <summary>
    /// Determines if there is an intersection between the current object and a <see cref="Sekai.Mathematics.Ray"/>.
    /// </summary>
    /// <param name="ray">The ray to test.</param>
    /// <param name="distance">When the method completes, contains the distance of the intersection,
    /// or 0 if there was no intersection.</param>
    /// <returns>Whether the two objects intersected.</returns>
    public bool Intersects(ref Ray ray, out float distance);

    /// <summary>
    /// Determines if there is an intersection between the current object and a <see cref="Sekai.Mathematics.Ray"/>.
    /// </summary>
    /// <param name="ray">The ray to test.</param>
    /// <param name="point">When the method completes, contains the point of intersection,
    /// or <see cref="Sekai.Mathematics.Vector3.Zero"/> if there was no intersection.</param>
    /// <returns>Whether the two objects intersected.</returns>
    public bool Intersects(ref Ray ray, out Vector3 point);
}