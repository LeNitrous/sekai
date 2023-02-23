// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Scenes;

/// <summary>
/// A three-dimensional scene.
/// </summary>
public sealed class Scene3D : Scene
{
    internal override RenderKind Kind => RenderKind.Render3D;

    public Scene3D()
    {
    }

    public Scene3D(Node root)
        : base(root)
    {
    }
}
