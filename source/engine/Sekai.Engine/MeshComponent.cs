// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Rendering;

namespace Sekai.Engine;

public class MeshComponent : Component
{
    public Mesh Mesh { get; set; } = null!;
}
