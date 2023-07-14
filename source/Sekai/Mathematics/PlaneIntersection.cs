// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Mathematics;

/// <summary>
/// The plane interseciton types.
/// </summary>
public enum PlaneIntersection
{
    /// <summary>
    /// The primitive is behind the plane.
    /// </summary>
    Back,

    /// <summary>
    /// The primitive is in front of the plane.
    /// </summary>
    Front,

    /// <summary>
    /// The primitive intersects the plane.
    /// </summary>
    Intersects,
}
