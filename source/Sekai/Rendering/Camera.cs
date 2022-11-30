// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Rendering;

public abstract class Camera : Component
{
    public Matrix4x4 ViewMatrix { get; internal set; }
    public Matrix4x4 ProjMatrix { get; internal set; }
}
