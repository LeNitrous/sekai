// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;
using Silk.NET.SDL;

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

    private void handleProcessEvent(Event evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Mousemotion:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLMouse mouse)
                            mouse.HandleEvent(evt.Motion);
                    }

                    break;
                }

            case EventType.Mousewheel:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLMouse mouse)
                            mouse.HandleEvent(evt.Wheel);
                    }

                    break;
                }

            case EventType.Mousebuttonup:
            case EventType.Mousebuttondown:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLMouse mouse)
                            mouse.HandleEvent(evt.Button);
                    }

                    break;
                }

            case EventType.Keyup:
            case EventType.Keydown:
                {
                    for (int i = 0; i < Available.Count; i++)
                    {
                        var device = Available[i];

                        if (device is SDLKeyboard keyboard)
                            keyboard.HandleEvent(evt.Key);
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
