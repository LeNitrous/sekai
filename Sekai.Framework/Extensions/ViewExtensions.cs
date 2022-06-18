// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;
using Silk.NET.Windowing;
using Veldrid;

namespace Sekai.Framework.Extensions;

internal static class ViewExtensions
{
    /// <summary>
    /// Creates a graphics context for the given view.
    /// </summary>
    public static IGraphicsContext CreateGraphics(this IView view, GraphicsBackend backend)
    {
        if (!view.IsInitialized)
            throw new InvalidOperationException(@"View must be initialized before a graphics context can be made.");

        GraphicsContext context = backend switch
        {
            GraphicsBackend.Vulkan => new VulkanGraphicsContext(),
            GraphicsBackend.OpenGL => new GLGraphicsContext(),
            GraphicsBackend.OpenGLES => new GLGraphicsContext(),
            GraphicsBackend.Direct3D11 => new Direct3D11GraphicsContext(),
            GraphicsBackend.Metal => throw new NotSupportedException(@"Metal is not supported."),
            _ => throw new ArgumentOutOfRangeException(nameof(backend)),
        };

        context.Initialize(view);
        return context;
    }
}
