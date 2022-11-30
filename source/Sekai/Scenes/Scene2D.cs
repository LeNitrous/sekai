// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A scene capable of drawing two-dimensional objects.
/// </summary>
public sealed class Scene2D : Scene<Node2D>, IRenderableScene
{
    public Camera2D? Camera { get; set; }

    public Scene2D()
    {
        Add<Transform2DProcessor>();
    }

    protected override Node CreateRootNode() => new Node2D();

    Camera? IRenderableScene.Camera => Camera;
}
