// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Rendering;

/// <summary>
/// A node capable of rendering to the scene.
/// </summary>
public interface IRenderableNode
{
    /// <summary>
    /// The node's transform.
    /// </summary>
    ITransform Transform { get; }
}
