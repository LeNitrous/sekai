// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.SDL;

public static class SDLGameBuilderExtensions
{
    public static GameBuilder<T> UseSDL<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        builder.UseView<SDLWindow>();
        return builder;
    }
}
