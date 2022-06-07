// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    private void destroy()
    {
        gsr.Unregister(this);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool isDisposing)
    {
        if (isDisposing) return;
        destroy();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}
