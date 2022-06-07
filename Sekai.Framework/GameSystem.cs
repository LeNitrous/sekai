// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.
using System;

namespace Sekai.Framework;
public abstract class GameSystem : IGameSystem
{
    public bool IsDisposed { get; private set; }
    private readonly GameSystemRegistry gsr;

    protected GameSystem(GameSystemRegistry gsr)
    {
        this.gsr = gsr;

        gsr.Register(this);
    }

    public virtual void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
        gsr.Unregister(this);
        GC.SuppressFinalize(this);
    }
}
