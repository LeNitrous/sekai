// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Scenes;
using Sekai.Services;

namespace Sekai;

/// <summary>
/// The building blocks of an entity which is capable of being attached to and add context to an entity.
/// </summary>
public abstract class Component
{
    /// <summary>
    /// The scene where this component is located in.
    /// </summary>
    public Scene Scene
    {
        get
        {
            if (!IsAttached)
                throw new InvalidOperationException(@"The component is not attached to an entity.");

            return scene;
        }
    }

    /// <summary>
    /// The entity owning this component.
    /// </summary>
    public Entity Entity
    {
        get
        {
            if (!IsAttached)
                throw new InvalidOperationException(@"The component is not attached to an entity.");

            return entity;
        }
    }

    private Scene scene = null!;
    private Entity entity = null!;
    internal bool HasStarted { get; private set; }
    internal bool IsAttached { get; private set; }

    /// <summary>
    /// Called on the first frame this component starts existing.
    /// </summary>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Called when the component has been attached to an entity.
    /// </summary>
    protected virtual void OnAttach()
    {
        Game.Resolve<ComponentService>().Add(this);
    }

    /// <summary>
    /// Called when the component is being detached from an entity.
    /// </summary>
    protected virtual void OnDetach()
    {
    }

    internal void Initialize()
    {
        if (HasStarted)
            return;

        Start();
        HasStarted = true;
    }

    internal void Attach(Scene scene, Entity entity)
    {
        if (IsAttached)
            return;

        this.scene = scene;
        this.entity = entity;

        IsAttached = true;
        OnAttach();
    }

    internal void Detach()
    {
        if (!IsAttached)
            return;

        OnDetach();

        scene = null!;
        entity = null!;
        IsAttached = false;
        HasStarted = false;
    }
}
