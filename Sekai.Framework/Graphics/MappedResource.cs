// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Extensions;
using Veldrid;

namespace Sekai.Framework.Graphics;

/// <summary>
/// A CPU-accessible region of GPU memory.
/// </summary>
internal abstract class MappedResource : GraphicsObject
{
    /// <summary>
    /// The mode that was used to map the resource.
    /// </summary>
    public MapMode Mode { get; }

    /// <summary>
    /// The size of the mapped region in bytes.
    /// </summary>
    public int Size => (int)Mapped.SizeInBytes;

    /// <summary>
    /// The pointer to this resource's data.
    /// </summary>
    public nint Data => Mapped.Data;

    protected readonly MappableResource Resource;
    protected readonly Veldrid.MappedResource Mapped;

    protected MappedResource(MappableResource resource, MapMode mode, int subresource = 0)
    {
        Mode = mode;
        Resource = resource;
        Mapped = Map(resource, mode, subresource);
    }

    protected virtual Veldrid.MappedResource Map(MappableResource resource, MapMode mode, int subresource) => Context.Device.Map(resource, mode.ToVeldrid(), (uint)subresource);
    protected sealed override void Destroy() => Context.Device.Unmap(Resource);
}
