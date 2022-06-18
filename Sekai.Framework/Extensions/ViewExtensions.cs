// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;
using Silk.NET.Windowing;

namespace Sekai.Framework.Extensions;

internal static class ViewExtensions
{
    /// <summary>
    /// Creates a graphics context for the given view.
    /// </summary>
    public static IGraphicsContext CreateGraphics(this IView view, Graphics.GraphicsAPI api)
    {
        if (!view.IsInitialized)
            throw new InvalidOperationException(@"View must be initialized before a graphics context can be made.");

        return new GraphicsContext(view, api);
    }
}
