// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;

namespace Sekai.Dummy;

public static class GameBuilderDummyExtensions
{
    public static GameBuilder<T> UseDummy<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .UseWindow<DummyWindow>()
            .UseGraphics<DummyGraphicsDevice>();
    }
}
