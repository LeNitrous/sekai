// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Skia;

public static class SkiaGameBuilderExtensions
{
    public static GameBuilder<T> UseSkia<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.ConfigureServices(services => services.Cache<SkiaContext>());
    }
}
