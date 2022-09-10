// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.Veldrid;

public static class VeldridHostExtensions
{
    public static HostBuilder<T> UseVeldrid<T>(this HostBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseGraphics<VeldridGraphicsDevice>();
    }
}
