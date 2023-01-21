// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Mathematics;

namespace Sekai.Rendering;

public interface IDrawable
{
    SortMode SortMode { get; }
    RenderGroup Group { get; }
    BoundingBox Bounds { get; }
    CullingMode Culling { get; }
    Transform Transform { get; }
    void Draw(RenderContext context);
}
