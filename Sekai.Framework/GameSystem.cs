// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework;
public abstract class GameSystem : FrameworkComponent, IGameSystem
{
    private readonly GameSystemRegistry gsr;

    protected GameSystem(GameSystemRegistry gsr)
    {
        this.gsr = gsr;

        gsr.Register(this);
    }

    protected override void Destroy()
    {
        gsr.Unregister(this);
    }
}
