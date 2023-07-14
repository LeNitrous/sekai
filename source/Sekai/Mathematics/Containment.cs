// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Mathematics;

/// <summary>
/// The containment types.
/// </summary>
public enum Containment
{
    /// <summary>
    /// Both primitives are disjointed.
    /// </summary>
    Disjoint,

    /// <summary>
    /// The left primitive contains the right primitive.
    /// </summary>
    Contains,

    /// <summary>
    /// Both primitives intersect each other.
    /// </summary>
    Intersects,
}
