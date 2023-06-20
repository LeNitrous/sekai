// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Windowing;

/// <summary>
/// Determines whether a given object has a renderable surface.
/// </summary>
public interface IHasSurface
{
    /// <summary>
    /// Called when the surface has been destroyed.
    /// </summary>
    event Action? SurfaceDestroyed;
}
