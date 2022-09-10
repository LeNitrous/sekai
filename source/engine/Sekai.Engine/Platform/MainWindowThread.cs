// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Threading;

namespace Sekai.Engine.Platform;

internal class MainWindowThread : FrameworkThread
{
    protected override bool PropagateExceptions => true;

    private readonly Action onNewFrame;

    public MainWindowThread(Action onNewFrame)
        : base(@"Main")
    {
        this.onNewFrame = onNewFrame;
    }

    protected override void OnNewFrame() => onNewFrame();
}
