// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A scene capable of drawing three-dimensional objects.
/// </summary>
public sealed class Scene3D : RenderableScene<Node3D>
{
    internal new Renderer3D Renderer => (Renderer3D)base.Renderer;

    protected override void Initialize(ProcessorCollection processors)
    {
        base.Initialize(processors);
        processors.Register<Camera3DProcessor>();
        processors.Register<Transform3DProcessor>();
    }

    protected sealed override Node CreateRootNode() => new Node3D();
    protected override Renderer CreateRenderer() => new Renderer3D();
}
