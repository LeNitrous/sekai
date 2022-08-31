// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Graphics;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

public class Mesh : FrameworkObject
{
    // <summary>
    /// Gets or sets this mesh's material.
    /// </summary>
    public Material Material { get; set; } = null!;

    /// <summary>
    /// Gets or sets this mesh's indices.
    /// </summary>
    public IndexBuffer IndexBuffer { get; set; } = null!;

    /// <summary>
    /// Gets or sets this mesh's vertices.
    /// </summary>
    public VertexBuffer VertexBuffer { get; set; } = null!;

    /// <summary>
    /// Gets or sets this mesh's topology.
    /// </summary>
    public PrimitiveTopology Topology { get; set; } = PrimitiveTopology.Triangles;

    /// <summary>
    /// Gets or sets how the mesh's faces will be culled.
    /// </summary>
    public FaceCulling Culling { get; set; } = FaceCulling.Back;

    /// <summary>
    /// Gets or sets the ordering of the mesh's vertices.
    /// </summary>
    public FaceWinding Winding { get; set; } = FaceWinding.Clockwise;
}
