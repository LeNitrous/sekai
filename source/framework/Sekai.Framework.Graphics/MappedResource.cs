// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public readonly struct MappedResource
{
    /// <summary>
    /// The mapped resource.
    /// </summary>
    public readonly IGraphicsResource Resource;

    /// <summary>
    /// Determines how this resource was mapped.
    /// </summary>
    public readonly MapMode Mode;

    /// <summary>
    /// The pointer to the start of the mapped data region.
    /// </summary>
    public readonly nint Data;

    /// <summary>
    /// The size of the mapped data region in bytes.
    /// </summary>
    public readonly uint Size;

    /// <summary>
    /// For mapped <see cref="INativeTexture"/>s, this is the subresource which is mapped to.
    /// </summary>
    public readonly uint Subresource;

    /// <summary>
    /// For mapped <see cref="INativeTexture"/>s, this is the number of bytes between each depth slice of a 3D texture.
    /// </summary>
    public readonly uint DepthPitch;

    /// <summary>
    /// For mapped <see cref="INativeTexture"/>s, this is the number of bytes between each row of texels.
    /// </summary>
    public readonly uint RowPitch;

    public MappedResource(IGraphicsResource resource, MapMode mode, nint data, uint size, uint subresource, uint depthPitch, uint rowPitch)
    {
        Resource = resource;
        Mode = mode;
        Data = data;
        Size = size;
        Subresource = subresource;
        DepthPitch = depthPitch;
        RowPitch = rowPitch;
    }
}
