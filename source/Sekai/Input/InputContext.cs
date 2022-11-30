// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input;

public class InputContext : FrameworkObject
{
    public event Action<IInputDevice, bool>? OnConnectionChanged;
    public IEnumerable<IInputDevice> Available => available;

    private readonly List<IInputDevice> available = new();

    public void Add(IInputDevice device)
    {
        if (available.Contains(device))
            return;

        available.Add(device);
        OnConnectionChanged?.Invoke(device, true);
    }

    public void Remove(IInputDevice device)
    {
        if (available.Remove(device))
            OnConnectionChanged?.Invoke(device, false);
    }
}
