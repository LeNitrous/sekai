// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Defines a mechanism for performing various color operations.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IColor<TSelf> : IFormattable, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>
    where TSelf : struct, IColor<TSelf>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>
{
}
