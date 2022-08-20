// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.Headless;

public static class HeadlessHostExtensions
{
    public static Host<T> UseHeadless<T>(this Host<T> host)
        where T : Game
    {
        return host
            .UseWindow<HeadlessWindow>()
            .UseGraphics<HeadlessGraphicsContext>();
    }
}
