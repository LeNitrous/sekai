// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Scenes;

/// <summary>
/// A two-dimensional scene.
/// </summary>
public sealed class Scene2D : Scene
{
    internal override RenderKind Kind => RenderKind.Render2D;

    public Scene2D()
    {
    }

    public Scene2D(Node root)
        : base(root)
    {
    }
}
