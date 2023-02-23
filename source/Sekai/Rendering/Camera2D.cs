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
public partial class Camera2D : Camera, IRenderObject
{
    /// <summary>
    /// The orthographic size of this camera.
    /// </summary>
    public Vector2 OrthoSize = Vector2.One;

    [Bind]
    internal Transform2D Transform { get; private set; } = null!;

    internal sealed override Transform GetTransform() => Transform;

    RenderKind IRenderObject.Kind => RenderKind.Render2D;
}
