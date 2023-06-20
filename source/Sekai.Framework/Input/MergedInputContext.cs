// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Input;

/// <summary>
/// An input context that merges other input contexts.
/// </summary>
public sealed class MergedInputContext : IInputContext
{
    public event Action<IInputDevice, bool>? ConnectionChanged;

    public IEnumerable<IInputDevice> Devices
    {
        get
        {
            for (int i = 0; i < contexts.Length; i++)
            {
                foreach (var device in contexts[i].Devices)
                {
                    yield return device;
                }
            }
        }
    }

    private bool isDisposed;
    private readonly IInputContext[] contexts;

    public MergedInputContext(params IInputContext[] contexts)
    {
        for (int i = 0; i < contexts.Length; i++)
        {
            contexts[i].ConnectionChanged += handleConnectionChanged;
        }

        this.contexts = contexts;
    }

    private void handleConnectionChanged(IInputDevice device, bool connected)
    {
        ConnectionChanged?.Invoke(device, connected);
    }

    ~MergedInputContext()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        for (int i = 0; i < contexts.Length; i++)
        {
            contexts[i].ConnectionChanged -= handleConnectionChanged;
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
