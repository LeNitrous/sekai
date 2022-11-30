// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Logging;

namespace Sekai;

internal class GameRunner : FrameworkObject
{
    [MemberNotNullWhen(true, "cts")]
    public bool IsRunning => cts is not null;

    private CancellationTokenSource? cts;
    private readonly Game game = Services.Current.Resolve<Game>();
    private readonly GraphicsContext graphics = Services.Current.Resolve<GraphicsContext>();

    public void Start()
    {
        if (IsRunning)
            return;

        cts = new CancellationTokenSource();

        double current = 0;
        double previous = 0;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while (true)
        {
            if (cts.Token.IsCancellationRequested)
                break;

            previous = current;
            current = stopwatch.Elapsed.Milliseconds;

            RunSingleFrame(current - previous);
        }

        cts = null;
        stopwatch.Stop();
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        cts.Cancel();

        if (!cts.Token.WaitHandle.WaitOne(10000))
            throw new TimeoutException("Failed to cancel in time.");
    }

    public void RunSingleFrame(double elapsed = 0)
    {
        graphics.View.DoEvents();

        try
        {
            game.Update(elapsed);

            graphics.Prepare();
            game.Render();
        }
        catch (Exception e)
        {
            Logger.Error(@"An exception has occured.", e);
        }
        finally
        {
            graphics.Present();
        }
    }
}
