// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A scene capable of drawing two-dimensional objects.
/// </summary>
public sealed class Scene2D : RenderableScene<Node2D>
{
    internal new Renderer2D Renderer => (Renderer2D)base.Renderer;

    protected override void Initialize(ProcessorCollection processors)
    {
        base.Initialize(processors);
        processors.Register<Camera2DProcessor>();
        processors.Register<Transform2DProcessor>();
    }

    protected override Node CreateRootNode() => new Node2D();
    protected override Renderer CreateRenderer() => new Renderer2D();
}
