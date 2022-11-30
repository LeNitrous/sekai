// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A scene capable of drawing three-dimensional objects.
/// </summary>
public sealed class Scene3D : Scene<Node3D>, IRenderableScene
{
    public Camera3D? Camera { get; set; }

    public Scene3D()
    {
        Add<Transform3DProcessor>();
    }

    protected sealed override Node CreateRootNode() => new Node3D();

    Camera? IRenderableScene.Camera => Camera;
}
