// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// A building block for <see cref="Scenes.Node"/> to define properties and behaviors for the given node that hosts this component.
/// </summary>
[Serializable]
public abstract class Component : ServiceableObject, IReferenceable
{
    public Guid Id { get; }

    /// <summary>
    /// The node owning this component.
    /// </summary>
    public Node? Node { get; private set; }

    /// <summary>
    /// The scene this component present in.
    /// </summary>
    public Scene? Scene => Node?.Scene;

    /// <summary>
    /// Gets whether this component is loaded or not.
    /// </summary>
    public bool IsLoaded
    {
        get => isLoaded;
        set
        {
            if (isLoaded == value)
                return;

            isLoaded = value;
            OnStateChanged?.Invoke(this, isLoaded);
        }
    }

    /// <summary>
    /// Invoked when the ready state has changed.
    /// </summary>
    public event Action<Component, bool>? OnStateChanged;

    private bool isLoaded;
    private readonly ComponentBinder binder;

    protected Component()
        : this(Guid.NewGuid())
    {
    }

    private Component(Guid id)
    {
        Id = id;
        binder = CreateBinder();
    }

    internal void Attach(Node node)
    {
        if (Node is not null)
            throw new InvalidOperationException($"{nameof(Component)} is already attached to a node.");

        Node = node;
        Node.OnComponentAdded += handleComponentAdded;
        Node.OnComponentRemoved += handleComponentRemoved;

        foreach (var component in Node.Components)
        {
            if (component == this)
                continue;

            binder.Bind(component);
        }

        IsLoaded = binder.IsComponentValid();
    }

    internal void Detach(Node node)
    {
        if (Node is null)
            return;

        if (Node != node)
            throw new InvalidOperationException($"Attempted to detach from a node that doesn't own this {nameof(Component)}.");

        Node.OnComponentAdded -= handleComponentAdded;
        Node.OnComponentRemoved -= handleComponentRemoved;
        Node = null;

        IsLoaded = false;
    }

    private void handleComponentAdded(object? sender, ComponentEventArgs args)
    {
        if (args.Component != this)
        {
            binder.Bind(args.Component);
        }
        
        IsLoaded = binder.IsComponentValid();
    }

    private void handleComponentRemoved(object? sender, ComponentEventArgs args)
    {
        if (args.Component != this)
        {
            binder.Unbind(args.Component);
        }
        
        IsLoaded = binder.IsComponentValid();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Node?.RemoveComponent(this);

        base.Dispose(disposing);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Overridden by source generators")]
    protected virtual ComponentBinder CreateBinder() => new DefaultComponentBinder(this);

    private class DefaultComponentBinder : ComponentBinder
    {
        public DefaultComponentBinder(Component owner)
            : base(owner, null!)
        {
        }

        public override bool Bind(Component other)
        {
            // Intentionally no-ops to prevent calling the base implementation.
            return false;
        }

        public override bool Unbind(Component other)
        {
            // Intentionally no-ops to prevent calling the base implementation.
            return false;
        }

        public override bool IsComponentValid()
        {
            // Intentionally no-ops to prevent calling the base implementation.
            return true;
        }

        protected override bool Update(Component other, bool isBinding)
        {
            // Intentionally no-ops to prevent calling the base implementation.
            return false;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Used by source generators")]
    protected abstract class ComponentBinder : DisposableObject
    {
        public readonly Component Owner;

        private List<Component>? bindings;
        private readonly ComponentBinder binder;

        protected ComponentBinder(Component owner, ComponentBinder binder)
        {
            Owner = owner;
            this.binder = binder;
        }

        public virtual bool Bind(Component other)
        {
            if (other == Owner)
                throw new ArgumentException("Cannot bind with itself.", nameof(other));

            if (binder.Bind(other))
                return true;

            bindings ??= new();

            if (bindings.Contains(other))
                return false;

            if (Update(other, true))
            {
                bindings.Add(other);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public virtual bool Unbind(Component other)
        {
            if (other == Owner)
                throw new ArgumentException("Cannot unbind from itself.", nameof(other));

            if (binder.Bind(other))
                return true;

            if (bindings is null)
                return false;

            if (!bindings.Remove(other))
                return false;

            return Update(other, false);
        }

        public virtual bool IsComponentValid()
        {
            return binder.IsComponentValid();
        }

        protected virtual bool Update(Component other, bool isBinding)
        {
            return binder.Update(other, isBinding);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Used by source generators")]
    protected abstract class ComponentBinder<T> : ComponentBinder
            where T : Component
    {
        public new T Owner => (T)base.Owner;

        protected ComponentBinder(T owner, ComponentBinder binder)
            : base(owner, binder)
        {
        }
    }
}
