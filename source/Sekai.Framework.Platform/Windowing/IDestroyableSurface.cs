// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Platform.Windowing;

/// <summary>
/// Determines whether a given object's surface is destroyable.
/// </summary>
public interface IDestroyableSurface
{
    /// <summary>
    /// Called when the surface has been destroyed.
    /// </summary>
    event Action? SurfaceDestroyed;
}
