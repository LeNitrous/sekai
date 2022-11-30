// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Scenes;

namespace Sekai.Rendering;

public class Camera3D : Camera
{
    public float NearPlane;
    public float FarPlane;
    public float AspectRatio;
    public float FieldOfView;

    internal override bool CanAttach(Node node) => node is Node3D;
}
