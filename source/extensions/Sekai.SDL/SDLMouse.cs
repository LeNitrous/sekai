// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Framework.Input;
using static SDL2.SDL;

namespace Sekai.SDL;

internal class SDLMouse : IMouse
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
    private readonly HashSet<MouseButton> pressedButtons = new();

    private Vector2 position;

    public Vector2 Position
    {
        get => position;
        set
        {
            if (view == null || position == value)
                return;

            position = value;
            SDL_WarpMouseInWindow(view.Window, (int)position.X, (int)position.Y);
        }
    }

    public SDLMouse(SDLView view)
    {
        this.view = view;
    }

    public void HandleEvent(SDL_MouseButtonEvent mouseButtonEvent)
    {
        var button = toMouseButton(mouseButtonEvent.button);

        switch (mouseButtonEvent.type)
        {
            case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                {
                    if (pressedButtons.Add(button))
                        OnMouseDown?.Invoke(this, button);
                    break;
                }

            case SDL_EventType.SDL_MOUSEBUTTONUP:
                {
                    if (pressedButtons.Remove(button))
                        OnMouseUp?.Invoke(this, (MouseButton)mouseButtonEvent.button);
                    break;
                }
        }
    }

    public void HandleEvent(SDL_MouseMotionEvent mouseMotionEvent)
    {
        position = new Vector2(mouseMotionEvent.x, mouseMotionEvent.y);
        OnMove?.Invoke(this, position);
    }

    public void HandleEvent(SDL_MouseWheelEvent mouseWheelEvent)
    {
        scrollWheels[0] = new ScrollWheel(mouseWheelEvent.x, mouseWheelEvent.y);
        OnScroll?.Invoke(this, scrollWheels[0]);
    }

    public bool IsButtonPressed(MouseButton button) => pressedButtons.Contains(button);

    private static MouseButton toMouseButton(uint button)
    {
        return button switch
        {
            SDL_BUTTON_LEFT => MouseButton.Left,
            SDL_BUTTON_RIGHT => MouseButton.Right,
            SDL_BUTTON_MIDDLE => MouseButton.Middle,
            SDL_BUTTON_X1 => MouseButton.Button4,
            SDL_BUTTON_X2 => MouseButton.Button5,
            _ => MouseButton.Unknown,
        };
    }
}
