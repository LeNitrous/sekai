// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.Headless.Windowing;

internal sealed class DummyMonitor : IMonitor
{
    public static IMonitor Instance = new DummyMonitor();

    public string Name { get; } = "Dummy";

    public int Index => 0;

    public Point Position => Point.Zero;

    public VideoMode Mode => new(Size.Zero, 0);

    private DummyMonitor()
    {
    }

    public IEnumerable<VideoMode> GetSupportedVideoModes()
    {
        yield return Mode;
    }
}
