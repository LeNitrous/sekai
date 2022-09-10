// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.Headless;

public static class HeadlessHostExtensions
{
    public static HostBuilder<T> UseHeadless<T>(this HostBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .UseWindow<HeadlessWindow>()
            .UseGraphics<HeadlessGraphicsDevice>();
    }
}
