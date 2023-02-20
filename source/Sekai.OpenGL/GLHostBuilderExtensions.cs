// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.OpenGL;

public static class GLHostBuilderExtensions
{
    /// <summary>
    /// Uses the OpenGL graphics system for this host.
    /// </summary>
    /// <returns>The host builder.</returns>
    public static IHostBuilder UseGL(this IHostBuilder builder)
    {
        return builder.UseGraphics(surface => new GLGraphicsSystem(surface));
    }
}
