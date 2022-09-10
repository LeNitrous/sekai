// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Engine.Threading;

namespace Sekai.Engine.Platform;

internal class MainUpdateThread : UpdateThread
{
    private readonly SystemCollection<GameSystem> systems;

    public MainUpdateThread(SystemCollection<GameSystem> systems)
        : base("Main")
    {
        this.systems = systems;
    }

    protected override void OnUpdateFrame(double delta)
    {
        foreach (var system in systems)
        {
            if (!system.IsAlive)
                continue;

            if (system is IUpdateable updateable)
                updateable.Update(delta);
        }
    }
}
