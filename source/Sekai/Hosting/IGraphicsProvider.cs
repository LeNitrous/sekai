// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.ComponentModel;
using Sekai.Graphics;
using Sekai.Windowing;

namespace Sekai.Hosting;

/// <summary>
/// A <see cref="GraphicsDevice"/> provider.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IGraphicsProvider
{
    /// <summary>
    /// Creates a <see cref="GraphicsDevice"/>.
    /// </summary>
    /// <param name="window">The window to attach the graphics device to.</param>
    GraphicsDevice CreateGraphics(IWindow window);
}
