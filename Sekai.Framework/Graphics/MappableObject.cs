// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Framework.Graphics;

/// <summary>
/// <see cref="GraphicsObject"/>s capable of having their resources mapped into the CPU address space.
/// </summary>
public abstract class MappableObject<T> : GraphicsObject<T>
    where T : DeviceResource
{
    /// <summary>
    /// Makes the contents of this mappable resource CPU-accessible.
    /// </summary>
    internal abstract MappedResource Map(MapMode mode);
}
