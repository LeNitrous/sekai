// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;

namespace Sekai.Rendering;

/// <summary>
/// A scene capable of rendering.
/// </summary>
public interface IRenderableScene
{
    /// <summary>
    /// Whether this scene is enabled or not.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Performs rendering operations using the graphics context.
    /// </summary>
    void Render(GraphicsContext graphicsContext);
}
