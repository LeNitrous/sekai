// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Mathematics;

/// <summary>
/// Defines a mechanism for computing intersections between <typeparamref name="TSelf"/> and <typeparamref name="TOther"/>.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
/// <typeparam name="TOther">The type that is computed against <typeparamref name="TSelf"/>.</typeparam>
/// <typeparam name="TResult">The type that is the result of the computation of the intersection.</typeparam>
public interface IIntersectable<TSelf, TOther, TResult>
    where TSelf : IIntersectable<TSelf, TOther, TResult>
{
    /// <summary>
    /// Test for intersection between a <typeparamref name="TSelf"/> and a <typeparamref name="TOther"/>.
    /// </summary>
    /// <param name="left">The value to which the <paramref name="right"/> is tested against.</param>
    /// <param name="right">The value to which the <paramref name="left"/> is tested against.</param>
    /// <returns>The result of the intersection between <paramref name="left"/> and <paramref name="right"/>.</returns>
    static abstract TResult Intersects(TSelf left, TOther right);
}
