// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A camera capable of rendering two-dimensional objects.
/// </summary>
[Processor<Camera2DProcessor>]
public class Camera2D : Camera
{
    /// <summary>
    /// The camera's orthographic size.
    /// </summary>
    public Vector2 OrthoSize = Vector2.One;

    /// <inheritdoc cref="Component.Owner"/>
    public new Node2D? Owner => (Node2D?)base.Owner;

    /// <inheritdoc cref="Component.Scene"/>
    public new Scene2D? Scene => (Scene2D?)base.Scene;

    protected override void OnActivate()
    {
        base.OnActivate();
        Scene?.Renderer.Add(this);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        Scene?.Renderer.Remove(this);
    }

    internal override bool CanAttach(Node node) => node is Node2D;
}
