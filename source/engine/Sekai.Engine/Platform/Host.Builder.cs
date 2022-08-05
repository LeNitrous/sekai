// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Threading;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Platform;

public partial class Host<T>
{
    private Type typeView = null!;
    private IView view = null!;
    private Action<T> callbackGameLoad = null!;
    private Action<ThreadController> callbackThreadController = null!;

    /// <summary>
    /// Uses the window of a given type.
    /// </summary>
    public Host<T> UseView<U>()
        where U : IView
    {
        typeView = typeof(U);
        return this;
    }

    /// <summary>
    /// Uses a callback that is invoked after the game has finished loading.
    /// </summary>
    public Host<T> UseLoadCallback(Action<T> callback)
    {
        callbackGameLoad = callback;
        return this;
    }

    /// <summary>
    /// Use a callback to prepare the thread controller.
    /// </summary>
    public Host<T> SetupThreadController(Action<ThreadController> callback)
    {
        callbackThreadController = callback;
        return this;
    }

    private void setupHostInstances()
    {
        if (typeView == null)
            throw new InvalidOperationException(@"Host does not have a window provided.");

        view = (IView)Activator.CreateInstance(typeView)!;
    }
}
