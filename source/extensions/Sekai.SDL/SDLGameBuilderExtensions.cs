// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Input;
using Sekai.Framework.Windowing;

namespace Sekai.SDL;

public static class SDLGameBuilderExtensions
{
    public static GameBuilder<T> UseSDL<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder
                .UseInput<SDLInputContext>()
                .UseWindow<SDLWindow>()
                .AddBuildAction(game =>
                {
                    var input = (SDLInputContext)game.Services.Resolve<IInputContext>();
                    var window = (SDLWindow)game.Services.Resolve<IView>();
                    input.Initialize(window);
                });
    }
}
