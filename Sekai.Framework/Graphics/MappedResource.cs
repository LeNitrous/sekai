// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Extensions;
using Sekai.Framework.Graphics.Buffers;
using Veldrid;

namespace Sekai.Framework.Graphics;

/// <summary>
/// A CPU-accessible region of GPU memory.
/// </summary>
internal unsafe class MappedResource : GraphicsObject
{
    /// <summary>
    /// The mode that was used to map the resource.
    /// </summary>
    public MapMode Mode { get;}

    /// <summary>
    /// The size of the mapped region in bytes.
    /// </summary>
    public int Size => (int)mapped.SizeInBytes;

    /// <summary>
    /// The pointer to this resource's data.
    /// </summary>
    public nint Data => mapped.Data;

    private readonly MappableResource resource;
    private readonly Veldrid.MappedResource mapped;

    internal MappedResource(Buffer buffer, MapMode mode)
        : this(buffer.Resource, mode)
    {
    }

    protected MappedResource(MappableResource resource, MapMode mode)
    {
        Mode = mode;
        mapped = CreateMappedResource(resource, mode);
        this.resource = resource;
    }

    protected sealed override void Destroy() => Context.Device.Unmap(resource);
    protected virtual Veldrid.MappedResource CreateMappedResource(MappableResource resource, MapMode mode) => Context.Device.Map(resource, mode.ToVeldrid());
}

/// <inheritdoc cref="MappedResource"/>
internal class MappedResource<T> : MappedResource
    where T : unmanaged
{
    private MappedResourceView<T> view;

    /// <summary>
    /// Gets the reference value at a given index.
    /// </summary>
    public ref T this[int i] => ref view[i];

    /// <summary>
    /// Number of elements contained in this resource.
    /// </summary>
    public int Count => view.Count;

    internal MappedResource(Buffer buffer, MapMode mode)
        : base(buffer, mode)
    {
    }

    protected override Veldrid.MappedResource CreateMappedResource(MappableResource resource, MapMode mode)
    {
        view = Context.Device.Map<T>(resource, mode.ToVeldrid());
        return view.MappedResource;
    }
}
