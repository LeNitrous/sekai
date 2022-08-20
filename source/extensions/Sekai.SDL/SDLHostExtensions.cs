// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.SDL;

public static class SDLHostExtensions
{
    public static Host<T> UseSDLWindow<T>(this Host<T> host)
        where T : Game
    {
        return host.UseWindow<SDLWindow>();
    }
}
