// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Mathematics;

/// <summary>
/// Defines a mechanism for computing collision between <typeparamref name="TSelf"/> and <typeparamref name="TOther"/>.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
/// <typeparam name="TOther">The type that is computed against <typeparamref name="TSelf"/>.</typeparam>
public interface IContainable<TSelf, TOther>
    where TSelf : IContainable<TSelf, TOther>
{
    /// <summary>
    /// Tests for collision between a <typeparamref name="TSelf"/> and a <typeparamref name="TOther"/>.
    /// </summary>
    /// <param name="left">The value to which the <paramref name="right"/> is tested against.</param>
    /// <param name="right">The value to which the <paramref name="left"/> is tested against.</param>
    /// <returns>The result of the collision test between <paramref name="left"/> and <paramref name="right"/>.</returns>
    static abstract Containment Contains(TSelf left, TOther right);
}
