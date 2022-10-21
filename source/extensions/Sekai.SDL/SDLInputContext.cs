// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework;
using Sekai.Framework.Input;
using static SDL2.SDL;

namespace Sekai.SDL;

internal class SDLInputContext : FrameworkObject, IInputContext
{
    public IReadOnlyList<IInputDevice> Available => available;
    public event Action<IInputDevice, bool> OnConnectionChanged = null!;
    private SDLView? view;
    private bool initialized;
    private readonly List<IInputDevice> available = new();

    public void Initialize(SDLView view)
    {
        if (initialized)
            return;

        initialized = true;

        this.view = view;
        AddInputDevice(new SDLMouse(view));
        AddInputDevice(new SDLKeyboard());
        this.view.OnProcessEvent += handleProcessEvent;
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

    private void handleProcessEvent(SDL_Event evt)
    {
        switch (evt.type)
        {
            case SDL_EventType.SDL_MOUSEMOTION:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLMouse mouse)
                            mouse.HandleEvent(evt.motion);
                    }

                    break;
                }

            case SDL_EventType.SDL_MOUSEWHEEL:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLMouse mouse)
                            mouse.HandleEvent(evt.wheel);
                    }

                    break;
                }

            case SDL_EventType.SDL_MOUSEBUTTONUP:
            case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLMouse mouse)
                            mouse.HandleEvent(evt.button);
                    }

                    break;
                }

            case SDL_EventType.SDL_KEYUP:
            case SDL_EventType.SDL_KEYDOWN:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLKeyboard keyboard)
                            keyboard.HandleEvent(evt.key);
                    }

                    break;
                }
        }
    }

    protected override void Destroy()
    {
        if (view != null)
            view.OnProcessEvent -= handleProcessEvent;
    }
}
