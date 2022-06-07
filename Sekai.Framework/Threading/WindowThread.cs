// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Silk.NET.Windowing;

namespace Sekai.Framework.Threading;

internal sealed class WindowThread : GameThread
{
    private readonly IView view;

    public WindowThread(IView view)
        : base("Window")
    {
        this.view = view;
        OnNewFrame += onNewFrame;
    }

    private void onNewFrame()
    {
        view.DoEvents();

        if (view.IsClosing)
            view.Reset();
    }
}
