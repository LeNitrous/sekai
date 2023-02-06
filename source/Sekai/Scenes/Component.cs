// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sekai.Extensions;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// A building block for <see cref="Scenes.Node"/> to define properties and behaviors for the given node that hosts this component.
/// </summary>
[Serializable]
public abstract class Component : DependencyObject, IReferenceable
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
    /// Gets whether this component has all bindings resolved or not.
    /// </summary>
    internal bool IsReady { get; private set; }

    /// <summary>
    /// Invoked when the ready state has changed.
    /// </summary>
    internal event Action<Component, bool>? OnStateChanged;

    private readonly List<ComponentBinding> bindings = new();

    protected Component()
        : this(Guid.NewGuid())
    {
        initialize(this);
    }

    private Component(Guid id)
    {
        Id = id;
    }

    internal void Attach(Node node)
    {
        if (Node is not null)
            throw new InvalidOperationException($"{nameof(Component)} is already attached to a node.");

        Node = node;
        Node.OnComponentAdded += handleComponentAdded;

        foreach ((var type, var info) in typeBindingMap[GetType()])
        {
            if (!Node.TryGetComponent(type, out var comp))
                continue;

            info.Create(this, comp).Bind();
        }

        updateReadyState();
    }

    internal void Detach(Node node)
    {
        if (Node is null)
            return;

        if (Node != node)
            throw new InvalidOperationException($"Attempted to detach from a node that doesn't own this {nameof(Component)}.");

        foreach (var binding in bindings)
            binding.Unbind();

        Node.OnComponentAdded -= handleComponentAdded;
        Node = null;
    }

    private void handleComponentAdded(object? sender, ComponentEventArgs args)
    {
        if (!typeBindingMap[GetType()].TryGetValue(args.Component.GetType(), out var info))
            return;

        info.Create(this, args.Component).Bind();

        updateReadyState();
    }

    private void updateReadyState()
    {
        bool current = stateEvaluators[GetType()].Invoke(this);

        if (current != IsReady)
        {
            IsReady = current;
            OnStateChanged?.Invoke(this, IsReady);
        }
    }

    protected override void Destroy() => Node?.RemoveComponent(this);

    private static void initialize(Component component)
    {
        var type = component.GetType();

        if (typeBindingMap.ContainsKey(type))
            return;

        var curr = type;
        var info = new List<PropertyInfo>();

        do
        {
            info.AddRange
            (
                curr
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(info => info.CanWrite && info.PropertyType.IsAssignableTo(typeof(Component)) && info.GetCustomAttribute<BindAttribute>(true) is not null)
            );
        }
        while ((curr = curr?.BaseType) is not null);

        var self = Expression.Parameter(typeof(Component), "this");

        Expression<Func<Component, bool>> lmbd;

        if (info.Count > 0)
        {
            var cond = info
                .Where(p => !p.IsNullable())
                .Select(i => Expression.IsTrue(Expression.NotEqual(Expression.Property(Expression.Convert(self, i.DeclaringType!), i), Expression.Constant(null))))
                .Aggregate<Expression>(Expression.And);

            lmbd = Expression.Lambda<Func<Component, bool>>(cond, false, self);
        }
        else
        {
            lmbd = Expression.Lambda<Func<Component, bool>>(Expression.Constant(true), false, self);
        }

        stateEvaluators.Add(type, lmbd.Compile());
        typeBindingMap.Add(type, info.ToDictionary(k => k.PropertyType, v => new BindingInfo(v)));
    }

    private static readonly Dictionary<Type, Func<Component, bool>> stateEvaluators = new();
    private static readonly Dictionary<Type, IReadOnlyDictionary<Type, BindingInfo>> typeBindingMap = new();

    private class BindingInfo
    {
        public Type SourceType => info.DeclaringType!;
        public Type TargetType => info.PropertyType;
        public bool Required => !info.IsNullable();

        private readonly PropertyInfo info;

        public BindingInfo(PropertyInfo info)
        {
            this.info = info;
        }

        public ComponentBinding Create(Component source, Component target)
            => new(this, source, target);

        public Action<Component, Component?> GetSetterAction()
        {
            if (!setterMap.TryGetValue(SourceType, out var targetMap))
                setterMap.Add(SourceType, targetMap = new());

            if (!targetMap.TryGetValue(TargetType, out var action))
            {
                var self = Expression.Parameter(typeof(Component), "this");
                var arg0 = Expression.Parameter(typeof(Component), "arg0");
                var prop = Expression.Property(Expression.Convert(self, SourceType), info);
                var body = Expression.Assign(prop, Expression.Convert(arg0, TargetType));
                var lmbd = Expression.Lambda<Action<Component, Component?>>(Expression.Block(body, Expression.Empty()), false, self, arg0);
                action = lmbd.Compile();
            }

            return action;
        }

        private static readonly Dictionary<Type, Dictionary<Type, Action<Component, Component?>>> setterMap = new();
    }

    private class ComponentBinding
    {
        private readonly Component source;
        private readonly Component target;
        private readonly BindingInfo info;

        public ComponentBinding(BindingInfo info, Component source, Component target)
        {
            this.info = info;
            this.source = source;
            this.target = target;
        }

        public void Bind()
        {
            source.bindings.Add(this);
            target.bindings.Add(this);
            info.GetSetterAction().Invoke(source, target);
        }

        public void Unbind()
        {
            source.bindings.Remove(this);
            target.bindings.Remove(this);
            info.GetSetterAction().Invoke(source, null);
        }
    }
}
