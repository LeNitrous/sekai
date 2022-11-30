// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Input;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal unsafe class SDLMouse : FrameworkObject, IMouse
{
    public IReadOnlyList<MouseButton> SupportedButtons { get; } = Enumerable.Range(0, 4).Select(n => (MouseButton)n).ToArray();
    public IReadOnlyList<ScrollWheel> ScrollWheels => scrollWheels;
    public string Name { get; } = @"Mouse";
    public int Index { get; } = 0;
    public bool IsConnected { get; } = true;
    public event Action<IMouse, MouseButton> OnMouseDown = null!;
    public event Action<IMouse, MouseButton> OnMouseUp = null!;
    public event Action<IMouse, ScrollWheel> OnScroll = null!;
    public event Action<IPointer, Vector2> OnMove = null!;
    private readonly SDLView view;
    private readonly ScrollWheel[] scrollWheels = new[] { new ScrollWheel() };
    private readonly List<MouseButton> pressedButtons = new();

    private Vector2 position;

    public Vector2 Position
    {
        get => position;
        set
        {
            if (view == null || position == value)
                return;

            position = value;
            view.Sdl.WarpMouseInWindow(view.Window, (int)position.X, (int)position.Y);
        }
    }

    public SDLMouse(SDLView view)
    {
        this.view = view;
        this.view.OnProcessEvent += onProcessEvent;
    }

    public bool IsButtonPressed(MouseButton button) => contains(button);

    private void handleEvent(MouseButtonEvent mouseButtonEvent)
    {
        var button = toMouseButton(mouseButtonEvent.Button);
        var type = (EventType)mouseButtonEvent.Type;

        switch (type)
        {
            case EventType.Mousebuttondown:
                {
                    if (!contains(button))
                    {
                        pressedButtons.Add(button);
                        OnMouseDown?.Invoke(this, button);
                    }
                    break;
                }

            case EventType.Mousebuttonup:
                {
                    if (pressedButtons.Remove(button))
                        OnMouseUp?.Invoke(this, (MouseButton)mouseButtonEvent.Button);
                    break;
                }
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

    private bool contains(MouseButton button)
    {
        for (int i = 0; i < pressedButtons.Count; i++)
        {
            if (pressedButtons[i] == button)
                return true;
        }

        return false;
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
        view.OnProcessEvent -= onProcessEvent;
    }
}
