// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Headless.Windowing;
using Sekai.Storages;
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

    public DummyPlatform(HostOptions options)
        : base(options)
    {
    }

    public override IWindow CreateWindow() => new DummyWindow();

    public override Storage CreateStorage()
    {
        var storage = new MountableStorage();

        storage.Mount(Storage.Game, new MemoryStorage(), false);
        storage.Mount(Storage.User, new MemoryStorage());
        storage.Mount(Storage.Temp, new MemoryStorage());

        return storage;
    }

    public override void Dispose()
    {
    }
}
