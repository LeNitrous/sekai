// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Mathematics;

/// <summary>
/// Defines a mechanism for merging two instances of a <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IMergeable<TSelf>
    where TSelf : IMergeable<TSelf>
{
    /// <summary>
    /// Merges two instances of a <typeparamref name="TSelf"/> to create a new <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="left">The value to which <paramref name="right"/> is merged with.</param>
    /// <param name="right">The value to which <paramref name="left"/> is merged with.</param>
    /// <returns>A merged <typeparamref name="TSelf"/>.</returns>
    static abstract TSelf Merge(TSelf left, TSelf right);
}
