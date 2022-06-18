// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Threading;

public class MainThread : FrameworkThread
{
    /// <summary>
    /// Action performed during <see cref="Perform"/>
    /// </summary>
    internal event Action? OnPerform;

    protected override bool PropagateExceptions => true;

    internal MainThread()
        : base(@"Main")
    {
    }

    protected sealed override void Perform() => OnPerform?.Invoke();
}
