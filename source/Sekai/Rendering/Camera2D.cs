// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Rendering;

public class Camera2D : Camera
{
    public Vector2 Zoom;

    internal override bool CanAttach(Node node) => node is Node2D;
}
