// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using Sekai.Collections;
using Sekai.Allocation;
using Sekai.Rendering;
using Sekai.Processors;

namespace Sekai.Scenes;

/// <summary>
/// A collection of nodes where it can process and render its children's components.
/// </summary>
public abstract partial class Scene : ServiceableObject
{
    /// <summary>
    /// The root node.
    /// </summary>
    public Node? Root
    {
        get => root;
        set
        {
            if (ReferenceEquals(root, value))
                return;

            if (root is not null)
                detachNode(root);

            root = value;

            if (root is not null)
                attachNode(root);
        }
    }

    /// <summary>
    /// The kind of renderer in use by this <see cref="Scene"/>.
    /// </summary>
    internal abstract RenderKind Kind { get; }

    private Node? root;
    private readonly List<Component> components = new();
    private readonly Dictionary<Uri, Node> nodes = new();
    private readonly Queue<ComponentEvent> events = new();

    [Resolved]
    private Renderer renderer { get; set; } = null!;

    [Resolved]
    private ProcessorManager processors { get; set; } = null!;

    protected Scene()
    {
    }

    protected Scene(Node root)
    {
        Root = root;
    }

    /// <summary>
    /// Gets a node using its path.
    /// </summary>
    /// <param name="path">The path to the node.</param>
    /// <returns></returns>
    public Node GetNode(string path)
    {
        if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri))
            throw new ArgumentException(@"Invalid path.", nameof(path));

        return GetNode(uri);
    }

    /// <summary>
    /// Gets a node using its URI.
    /// </summary>
    /// <param name="uri">The URI to the node.</param>
    /// <returns></returns>
    public Node GetNode(Uri uri)
    {
        if (uri.IsAbsoluteUri && uri.Scheme != Node.Scheme)
            throw new ArgumentException(@"URI has an invalid scheme.", nameof(uri));

        Node? node = null;

        foreach (var pair in nodes)
        {
            if (UriEqualityComparer.Default.Equals(pair.Key, uri))
            {
                node = pair.Value;
                break;
            }
        }

        if (node is null)
            throw new NodeNotFoundException(@"Node not found.");

        return node;
    }

    /// <summary>
    /// Retrieves nodes containing a given tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>An enumeration of nodes.</returns>
    public IEnumerable<Node> GetNodes(string tag)
    {
        foreach (var node in nodes.Values)
        {
            if (node.Tags.Contains(tag))
                yield return node;
        }
    }

    /// <summary>
    /// Updates the scene.
    /// </summary>
    public void Update()
    {
        while (events.TryDequeue(out var e))
        {
            switch (e.Type)
            {
                case ComponentEventType.Attach:
                    processors.Attach(e.Component);
                    components.Add(e.Component);
                    break;

                case ComponentEventType.Detach:
                    processors.Detach(e.Component);
                    components.Remove(e.Component);
                    break;
            }
        }

        renderer.Begin(Kind);

        foreach (var component in CollectionsMarshal.AsSpan(components))
            processors.Update(component);

        renderer.End();
    }

    /// <summary>
    /// Renders the scene.
    /// </summary>
    public void Render()
    {
        renderer.Flush();
    }

    private void attachNode(Node node)
        => handleNodeAdded(this, new(node));

    private void detachNode(Node node)
        => handleNodeRemoved(this, new(node));

    private void addComponent(Component component, bool immediate = false)
    {
        component.OnStateChanged += handleComponentStateChange;

        if (immediate)
            events.Enqueue(new(ComponentEventType.Attach, component));
    }

    private void remComponent(Component component, bool immediate = false)
    {
        component.OnStateChanged -= handleComponentStateChange;

        if (immediate)
            events.Enqueue(new(ComponentEventType.Detach, component));
    }

    private void handleComponentStateChange(Component component, bool state)
    {
        if (state)
        {
            events.Enqueue(new(ComponentEventType.Attach, component));
        }
        else
        {
            events.Enqueue(new(ComponentEventType.Detach, component));
        }
    }

    private void handleNodeAdded(object? sender, NodeEventArgs args)
    {
        var node = args.Node;

        node.Scene = this;
        node.OnNodeAdded += handleNodeAdded;
        node.OnNodeRemoved += handleNodeRemoved;
        node.OnComponentAdded += handleComponentAdded;
        node.OnComponentRemoved += handleComponentRemoved;

        foreach (var component in node.Components)
            addComponent(component, component.IsLoaded);

        nodes.Add(node.Uri, node);

        foreach (var child in node)
        {
            if (nodes.ContainsKey(child.Uri))
                continue;

            foreach (var component in child.Components)
                addComponent(component, component.IsLoaded);

            nodes.Add(child.Uri, child);
        }
    }

    private void handleNodeRemoved(object? sender, NodeEventArgs args)
    {
        var node = args.Node;

        node.Scene = null;
        node.OnNodeAdded -= handleNodeAdded;
        node.OnNodeRemoved -= handleNodeRemoved;
        node.OnComponentAdded -= handleComponentAdded;
        node.OnComponentRemoved -= handleComponentRemoved;

        foreach (var child in node)
        {
            if (!nodes.Remove(child.Uri))
                continue;

            foreach (var component in child.Components)
                remComponent(component, component.IsLoaded);
        }

        foreach (var component in node.Components)
            remComponent(component, true);

        nodes.Remove(node.Uri);
    }

    private void handleComponentAdded(object? sender, ComponentEventArgs args)
        => addComponent(args.Component);

    private void handleComponentRemoved(object? sender, ComponentEventArgs args)
        => remComponent(args.Component);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (root is not null)
                detachNode(root);
        }

        base.Dispose(disposing);
    }

    private readonly record struct ComponentEvent(ComponentEventType Type, Component Component);

    private enum ComponentEventType
    {
        Attach,
        Detach,
    }
}

/// <summary>
/// An exception thrown when a node is not found at a given path.
/// </summary>
public class NodeNotFoundException : Exception
{
    public NodeNotFoundException(string? message)
        : base(message)
    {
    }
}
