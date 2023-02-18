// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.SDL;

public static class SDLHostBuilderExtensions
{
    /// <summary>
    /// Uses SDL as the surface system.
    /// </summary>
    /// <returns>The host builder.</returns>
    public static IHostBuilder UseSDL(this IHostBuilder builder)
    {
        var surface = RuntimeInfo.IsDesktop ? new SDLWindow() : new SDLSurface();
        var input = new SDLInputSystem(surface);

        builder.UseSurface(surface);
        builder.UseInput(input);

        return builder;
    }
}
