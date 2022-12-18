// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A three-dimensional <see cref="Drawable"/>.
/// </summary>
public abstract class Drawable3D : Drawable
{
    /// <inheritdoc cref="Component.Owner"/>
    public new Node3D? Owner => (Node3D?)base.Owner;

    /// <inheritdoc cref="Component.Scene"/>
    public new Scene3D? Scene => (Scene3D?)base.Scene;

    /// <inheritdoc cref="Drawable.Transform"/>
    public new Transform3D Transform => (Transform3D)base.Transform;

    /// <inheritdoc cref="Drawable.Draw"/>
    public abstract void Draw(Renderer3D renderer);

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

    internal override bool CanAttach(Node node) => base.CanAttach(node) && node is Node3D;

    internal override void Draw(Renderer renderer)
    {
        if (renderer is not Renderer3D renderer3D)
            throw new InvalidOperationException(@"The renderer is not a 3D renderer.");

        Draw(renderer3D);
    }
}
