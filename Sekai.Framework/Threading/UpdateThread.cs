// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Threading;

internal class UpdateThread : GameThread
{
    private double lastFrameTime;
    private readonly Action<double>? onUpdate;

    public UpdateThread(Action<double>? onUpdate = null)
        : base("Update")
    {
        OnNewFrame += onNewFrame;
        this.onUpdate = onUpdate;
    }

    private void onNewFrame()
    {
        onUpdate?.Invoke(CurrentTime - lastFrameTime);
        lastFrameTime = CurrentTime;
    }
}
