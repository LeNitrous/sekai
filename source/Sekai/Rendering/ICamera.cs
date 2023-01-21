// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Graphics.Textures;
using Sekai.Mathematics;

namespace Sekai.Rendering;

public interface ICamera
{
    RenderGroup Groups { get; }
    RenderTarget? Target { get; }
    BoundingFrustum Frustum { get; }
    Matrix4x4 ViewMatrix { get; }
    Matrix4x4 ProjMatrix { get; }
    Transform Transform { get; }
}
