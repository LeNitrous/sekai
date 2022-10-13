// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;

namespace Sekai.SDL;

public static class SDLGameBuilderExtensions
{
    public static GameBuilder<T> UseSDLWindow<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseWindow<SDLWindow>();
    }
}
