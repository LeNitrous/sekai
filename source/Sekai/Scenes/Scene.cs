// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sekai.Allocation;
using Sekai.Processors;

namespace Sekai.Scenes;

public class Scene : ActivateableObject
{
    public readonly Node Root;
    public readonly Services Services = Services.Current.CreateScoped();
    public SceneCollection? Scenes { get; private set; }
    private readonly Dictionary<Type, Processor> processors = new();
    private readonly List<IRenderable> renderables = new();
    private readonly List<IUpdateable> updateables = new();

    public Scene()
    {
        Root = CreateRootNode();
        Add<BehaviorProcessor>();
    }

    protected void Add<T>()
        where T : Processor, new()
    {
        var processor = Activator.CreateInstance<T>();

        if (processor is IRenderable renderable)
            renderables.Add(renderable);

        if (processor is IUpdateable updateable)
            updateables.Add(updateable);

        processors.Add(typeof(T), processor);
    }

    public T Get<T>()
        where T : Processor, new()
    {
        if (!processors.TryGetValue(typeof(T), out var processor))
            throw new KeyNotFoundException();

        return Unsafe.As<T>(processor);
    }

    internal void Render()
    {
        foreach (var renderable in renderables)
        {
            if (!renderable.Enabled || !renderable.IsAttached)
                continue;

            renderable.Render();
        }
    }

    internal void Update(double elapsed)
    {
        foreach (var updateable in updateables)
        {
            if (!updateable.Enabled || !updateable.IsAttached)
                continue;

            updateable.Update(elapsed);
        }
    }

    protected override void OnAttach()
    {
        foreach (var processor in processors)
            processor.Value.Attach(this);
    }

    protected override void OnDetach()
    {
        foreach (var processor in processors)
            processor.Value.Detach(this);
    }

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
