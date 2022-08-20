// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.Veldrid;

public static class VeldridHostExtensions
{
    public static Host<T> UseVeldrid<T>(this Host<T> host)
        where T : Game
    {
        return host.UseGraphics<VeldridGraphicsContext>();
    }
}
