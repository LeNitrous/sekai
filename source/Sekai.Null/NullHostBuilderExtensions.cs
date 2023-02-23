// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Null.Audio;
using Sekai.Null.Graphics;
using Sekai.Null.Windowing;

namespace Sekai.Null;

public static class NullHostBuilderExtensions
{
    /// <summary>
    /// Uses null services.
    /// </summary>
    /// <returns>
    /// The host builder.
    /// </returns>
    public static IHostBuilder UseNull(this IHostBuilder builder)
    {
        return builder.UseNullGraphics().UseNullSurface().UseNullAudio();
    }

    /// <summary>
    /// Uses a null graphics system.
    /// </summary>
    /// <returns>
    /// The host builder.
    /// </returns>
    public static IHostBuilder UseNullGraphics(this IHostBuilder builder)
    {
        return builder.UseGraphics(surface => new NullGraphicsSystem(surface));
    }

    /// <summary>
    /// Uses a null surface.
    /// </summary>
    /// <returns>
    /// The host builder.
    /// </returns>
    public static IHostBuilder UseNullSurface(this IHostBuilder builder)
    {
        return builder.UseSurface<NullSurface>();
    }

    /// <summary>
    /// Uses a null audio system.
    /// </summary>
    /// <returns>
    /// The host builder.
    /// </returns>
    public static IHostBuilder UseNullAudio(this IHostBuilder builder)
    {
        return builder.UseAudio<NullAudioSystem>();
    }
}
