// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A container for all <see cref="Node"/>s.
/// </summary>
public class Scene : ActivateableObject
{
    /// <summary>
    /// The root node of this scene.
    /// </summary>
    public readonly Node Root;

    /// <summary>
    /// The scene services.
    /// </summary>
    /// <remarks>
    /// Services registered are only local to this scene and does not affect the global <see cref="Allocation.Services"/>.
    /// </remarks>
    public readonly ServiceContainer Services = Allocation.Services.CreateScoped();

    /// <summary>
    /// The scene collection where this scene is currently attached to.
    /// </summary>
    public SceneCollection? Scenes { get; private set; }

    internal readonly ProcessorCollection Processors = new();

    public Scene()
    {
        Root = CreateRootNode();
        Initialize(Processors);
    }

    protected virtual void Initialize(ProcessorCollection processors)
    {
        processors.Register<ScriptProcessor>();
        processors.Register<BehaviorProcessor>();
    }

    internal void Update(double delta)
    {
        foreach (var processor in Processors)
            processor.Update(delta);
    }

    protected override void OnAttach() => Processors.Attach(this);

    protected override void OnDetach() => Processors.Detach(this);

    internal void Attach(SceneCollection scenes)
    {
        if (IsAttached)
            return;

        if (scenes is null)
            throw new InvalidOperationException();

        Scenes = scenes;

        Attach();
        Root.Attach(this);
    }

    internal void Detach(SceneCollection scenes)
    {
        if (!IsAttached)
            return;

        if (Scenes != scenes)
            throw new InvalidOperationException();

        Scenes = null;

        Root.Detach(this);
        Detach();
    }

    protected virtual Node CreateRootNode() => new();
}

public abstract class Scene<T> : Scene
    where T : Node
{
    public new T Root => (T)base.Root;
}

public abstract class RenderableScene<T> : Scene<T>, IRenderableScene
    where T : Node, IRenderableNode
{
    internal Renderer Renderer { get; }

    public RenderableScene()
    {
        Renderer = CreateRenderer();
    }

    public void Render(GraphicsContext graphics) => Renderer.Render(graphics);

    protected abstract Renderer CreateRenderer();
}
