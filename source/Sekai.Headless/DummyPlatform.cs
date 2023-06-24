// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Headless.Windowing;
using Sekai.Windowing;

namespace Sekai.Headless;

internal sealed class DummyPlatform : Platform
{
    public override IMonitor PrimaryMonitor => DummyMonitor.Instance;

    public override IEnumerable<IMonitor> Monitors
    {
        get
        {
            yield return DummyMonitor.Instance;
        }
    }

    public override IWindow CreateWindow() => new DummyWindow();

    public override void Dispose()
    {
    }
}
