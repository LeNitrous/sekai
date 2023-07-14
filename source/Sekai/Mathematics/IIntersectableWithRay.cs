// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Defines a mechanism for computing intersections between <typeparamref name="TSelf"/> and <see cref="Ray"/>.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IIntersectableWithRay<TSelf> : IIntersectable<TSelf, Ray, bool>
    where TSelf : IIntersectableWithRay<TSelf>
{
    /// <summary>
    /// Test for intersection between a <typeparamref name="TSelf"/> and a <see cref="Ray"/>.
    /// </summary>
    /// <param name="self">The value to which the <paramref name="ray"/> is tested against.</param>
    /// <param name="ray">The ray which the <paramref name="self"/> is tested against.</param>
    /// <param name="point">The point where the intersection occurs.</param>
    /// <returns>The result of the intersection between <paramref name="self"/> and <paramref name="ray"/>.</returns>
    static abstract bool Intersects(TSelf self, Ray ray, out Vector3 point);

    /// <summary>
    /// Test for intersection between a <typeparamref name="TSelf"/> and a <see cref="Ray"/>.
    /// </summary>
    /// <param name="self">The value to which the <paramref name="ray"/> is tested against.</param>
    /// <param name="ray">The ray which the <paramref name="self"/> is tested against.</param>
    /// <param name="distance">The distance of intersection between <paramref name="self"/> and <paramref name="ray"/>.</param>
    /// <returns>The result of the intersection between <paramref name="self"/> and <paramref name="ray"/>.</returns>
    static abstract bool Intersects(TSelf self, Ray ray, out float distance);
}
