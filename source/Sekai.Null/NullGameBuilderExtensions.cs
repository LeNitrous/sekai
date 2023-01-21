// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Null.Audio;
using Sekai.Null.Graphics;
using Sekai.Null.Windowing;

namespace Sekai.Null;

public static class NullGameBuilderExtensions
{
    /// <summary>
    /// Uses null services.
    /// </summary>
    public static GameBuilder<T> UseNull<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .UseNullGraphics()
            .UseNullSurface()
            .UseNullAudio();
    }

    /// <summary>
    /// Uses a null graphics system.
    /// </summary>
    public static GameBuilder<T> UseNullGraphics<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseGraphics<NullGraphicsSystem>();
    }

    /// <summary>
    /// Uses a null surface.
    /// </summary>
    public static GameBuilder<T> UseNullSurface<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseSurface<NullSurface>();
    }

    public static GameBuilder<T> UseNullAudio<T>(this GameBuilder<T> builder)
        where T: Game, new()
    {
        return builder.UseAudio<NullAudioSystem>();
    }
}
