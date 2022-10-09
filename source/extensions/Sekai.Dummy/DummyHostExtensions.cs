// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine;
using Sekai.Engine.Platform;

namespace Sekai.Dummy;

public static class DummyHostExtensions
{
    public static HostBuilder<T> UseDummy<T>(this HostBuilder<T> builder)
        where T : Game, new()
    {
        return builder
            .UseWindow<DummyWindow>()
            .UseGraphics<DummyGraphicsDevice>();
    }
}
