// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;

namespace Sekai.Veldrid;

public static class VeldridGameBuilderExtensions
{
    public static GameBuilder<T> UseVeldrid<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseGraphics<VeldridGraphicsDevice>();
    }
}
