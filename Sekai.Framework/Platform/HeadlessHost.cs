// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public class HeadlessHost : Host
{
    private GameThread? mainThread;
    private readonly Queue<Action> deferredDispatchQueue = new();

    public void Dispatch(Action action)
    {
        if (mainThread == null)
        {
            deferredDispatchQueue.Enqueue(action);
            return;
        }

        mainThread.Dispatch(action);
    }

    protected override void Initialize(Game game)
    {
        while (deferredDispatchQueue.TryDequeue(out var action))
            mainThread!.Dispatch(action);
    }

    protected override GameThread CreateMainThread()
    {
        return mainThread = new GameThread("Main");
    }
}
