// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input;
using Sekai.Windowing;

namespace Sekai.SDL;

public static class SDLGameBuilderExtensions
{
    public static GameBuilder<T> UseSDL<T>(this GameBuilder<T> builder, bool useInput = true)
        where T : Game, new()
    {
        builder.UseWindow<SDLWindow>();

        if (useInput)
        {
            builder
                .UseInput<SDLInputContext>()
                .AddBuildAction(game =>
                {
                    var input = (SDLInputContext)Game.Resolve<IInputContext>();
                    var window = (SDLWindow)Game.Resolve<IView>();
                    input.Initialize(window);
                });
        }

        return builder;
    }
}
