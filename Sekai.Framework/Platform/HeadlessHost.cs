// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Platform;

public class HeadlessHost : Host
{
    private bool isReady;
    private Task? runTask;
    private GameThread? mainThread;

    protected sealed override GameThread CreateMainThread()
    {
        return mainThread = new GameThread("Main");
    }

    protected override void Run()
    {
        mainThread?.Dispatch(() => isReady = true);

        runTask = Task.Factory.StartNew(() => base.Run(), TaskCreationOptions.LongRunning);

        while (!isReady)
            Thread.Sleep(10);
    }

    protected override void Destroy()
    {
        base.Destroy();
        runTask?.Wait();
        CheckForExceptions();
    }

    protected void CheckForExceptions()
    {
        if (runTask != null && runTask.Exception != null)
            throw runTask.Exception;
    }
}
