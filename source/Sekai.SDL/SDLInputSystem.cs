// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Allocation;
using Sekai.Input;
using Sekai.Windowing;

namespace Sekai.SDL;

internal sealed class SDLInputSystem : InputSystem
{
    public override IReadOnlyList<IInputDevice> Connected => connected;

    public override event Action<IInputDevice, bool>? OnConnectionChanged;

    [Resolved]
    private Surface gameSurface { get; set; }

    private readonly List<IInputDevice> connected = new();

    public SDLInputSystem()
    {
        if (gameSurface is not SDLSurface s)
            throw new InvalidOperationException($"Surface must be an {nameof(SDLSurface)}.");

        connected.Add(new SDLMouse(s));
        connected.Add(new SDLKeyboard(s));
    }
}
