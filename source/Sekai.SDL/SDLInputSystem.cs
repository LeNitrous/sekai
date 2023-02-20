// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;

namespace Sekai.SDL;

internal sealed class SDLInputSystem : InputSystem
{
    public override IReadOnlyList<IInputDevice> Connected => connected;

    public override string Name => surface.Name;

    public override Version Version => surface.Version;

    // TODO: Support SDL Controllers
#pragma warning disable 0067
    public override event Action<IInputDevice, bool>? OnConnectionChanged;
#pragma warning restore 0067

    private readonly SDLSurface surface;
    private readonly List<IInputDevice> connected = new();

    public SDLInputSystem(SDLSurface surface)
    {
        this.surface = surface;
        connected.Add(new SDLMouse(surface));
        connected.Add(new SDLKeyboard(surface));
    }
}
