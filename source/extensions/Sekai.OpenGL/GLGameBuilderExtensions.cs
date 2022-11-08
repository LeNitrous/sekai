// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;

namespace Sekai.OpenGL;

public static class GLGameBuilderExtensions
{
    public static GameBuilder<T> UseGL<T>(this GameBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseGraphics<GLGraphicsSystem>();
    }
}
