// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Input.Devices.Pointers;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal unsafe class SDLMouse : FrameworkObject, IMouse
{
    public string Name { get; } = @"Mouse";
    public IReadOnlyList<ScrollWheel> ScrollWheels => scrollWheels;
    public IReadOnlyList<MouseButton> Buttons { get; } = Enumerable.Range(0, 4).Select(n => (MouseButton)n).ToArray();

    public event Action<IMouse, MouseButton>? OnButtonPressed;
    public event Action<IMouse, MouseButton>? OnButtonRelease;
    public event Action<IMouse, ScrollWheel>? OnScroll;
    public event Action<IPointer, Vector2>? OnMove;

    private Vector2 position;
    private readonly SDLSurface surface;
    private readonly ScrollWheel[] scrollWheels = new ScrollWheel[1];

    public Vector2 Position
    {
        get => position;
        set
        {
            if (surface == null || position == value)
                return;

            position = value;
            surface.Invoke(() => surface.Sdl.WarpMouseInWindow(surface.Window, (int)position.X, (int)position.Y));
        }
    }

    public SDLMouse(SDLSurface surface)
    {
        this.surface = surface;
        this.surface.OnProcessEvent += onProcessEvent;
    }

    private void handleEvent(MouseButtonEvent mouseButtonEvent)
    {
        var button = toMouseButton(mouseButtonEvent.Button);
        var type = (EventType)mouseButtonEvent.Type;

        switch (type)
        {
            case EventType.Mousebuttondown:
                OnButtonPressed?.Invoke(this, button);
                break;

            case EventType.Mousebuttonup:
                OnButtonRelease?.Invoke(this, button);
                break;
        }
    }

    private void handleEvent(MouseMotionEvent mouseMotionEvent)
    {
        position = new Vector2(mouseMotionEvent.X, mouseMotionEvent.Y);
        OnMove?.Invoke(this, position);
    }

    private void handleEvent(MouseWheelEvent mouseWheelEvent)
    {
        scrollWheels[0] = new ScrollWheel(mouseWheelEvent.X, mouseWheelEvent.Y);
        OnScroll?.Invoke(this, scrollWheels[0]);
    }

    private void onProcessEvent(Event evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Mousemotion:
                handleEvent(evt.Motion);
                break;

            case EventType.Mousewheel:
                handleEvent(evt.Wheel);
                break;

            case EventType.Mousebuttonup:
            case EventType.Mousebuttondown:
                handleEvent(evt.Button);
                break;
        }
    }

    private static MouseButton toMouseButton(uint button)
    {
        return button switch
        {
            Sdl.ButtonLeft => MouseButton.Left,
            Sdl.ButtonRight => MouseButton.Right,
            Sdl.ButtonMiddle => MouseButton.Middle,
            Sdl.ButtonX1 => MouseButton.Button4,
            Sdl.ButtonX2 => MouseButton.Button5,
            _ => MouseButton.Unknown,
        };
    }

    protected override void Destroy()
    {
        surface.OnProcessEvent -= onProcessEvent;
    }
}
