// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.SDL;

public static class SDLHostExtensions
{
    public static HostBuilder<T> UseSDLWindow<T>(this HostBuilder<T> builder)
        where T : Game, new()
    {
        return builder.UseWindow<SDLWindow>();
    }
}
