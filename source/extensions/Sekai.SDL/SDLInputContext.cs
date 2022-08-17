// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Input;

namespace Sekai.SDL;

internal class SDLInputContext : IInputContext
{
    public IReadOnlyList<IInputDevice> Available => available;
    public event Action<IInputDevice, bool> OnConnectionChanged = null!;
    private readonly List<IInputDevice> available = new();

    public SDLInputContext(SDLView view)
    {
        AddInputDevice(new SDLMouse(view));
        AddInputDevice(new SDLKeyboard());
    }

    public void AddInputDevice(IInputDevice device)
    {
        if (available.Contains(device))
            return;

        available.Add(device);
        OnConnectionChanged?.Invoke(device, true);
    }

    public void RemoveInputDevice(IInputDevice device)
    {
        if (!available.Contains(device))
            return;

        available.Remove(device);
        OnConnectionChanged?.Invoke(device, false);
    }
}
