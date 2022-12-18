// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A two-dimensional <see cref="Drawable"/>.
/// </summary>
public abstract class Drawable2D : Drawable
{
    /// <inheritdoc cref="Component.Owner"/>
    public new Node2D? Owner => (Node2D?)base.Owner;

    /// <inheritdoc cref="Component.Scene"/>
    public new Scene2D? Scene => (Scene2D?)base.Scene;

    /// <inheritdoc cref="Drawable.Transform"/>
    public new Transform2D Transform => (Transform2D)base.Transform;

    /// <inheritdoc cref="Drawable.Draw"/>
    public abstract void Draw(Renderer2D renderer);

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

    internal override bool CanAttach(Node node) => base.CanAttach(node) && node is Node2D;

    internal override void Draw(Renderer renderer)
    {
        if (renderer is not Renderer2D renderer2D)
            throw new InvalidOperationException(@"The renderer is not a 2D renderer.");

         Draw(renderer2D);
    }
}
