// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Allocation;

public readonly struct ValueInvokeOnDisposal : IDisposable
{
    private readonly Action action;

    public ValueInvokeOnDisposal(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        this.action = action;
    }

    public void Dispose() => action();
}
