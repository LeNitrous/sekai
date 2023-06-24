// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Input;

namespace Sekai.Desktop.Input;

internal sealed unsafe class Mouse : IMouse
{
    public string Name => name;
    public bool IsConnected => true;
    public IReadOnlyList<MouseButton> Buttons => buttons;
    public IReadOnlyList<ScrollWheel> ScrollWheels => scrollWheels;

    public Vector2 Position
    {
        get
        {
            glfw.GetCursorPos(window, out double x, out double y);
            return new((float)x, (float)y);
        }
        set => glfw.SetCursorPos(window, value.X, value.Y);
    }

    public event Action<IMouse, ScrollWheel>? OnScroll;
    public event Action<IMouse, MouseButton, bool>? OnButton;
    public event Action<IPointer, Vector2>? OnMove;

    private readonly ScrollWheel[] scrollWheels = new ScrollWheel[1];
    private readonly Silk.NET.GLFW.Glfw glfw;
    private readonly Silk.NET.GLFW.WindowHandle* window;

    public Mouse(Silk.NET.GLFW.Glfw glfw, Silk.NET.GLFW.WindowHandle* window)
    {
        this.glfw = glfw;
        this.window = window;
    }

#pragma warning disable IDE0060

    public void HandleMouseScroll(Silk.NET.GLFW.WindowHandle* window, double x, double y)
    {
        var scroll = new ScrollWheel((float)x, (float)y);

        if (scroll.Equals(scrollWheels[0]))
        {
            return;
        }

        OnScroll?.Invoke(this, scrollWheels[0] = scroll);
    }

    public void HandleMouseMotion(Silk.NET.GLFW.WindowHandle* window, double x, double y)
    {
        OnMove?.Invoke(this, new((float)x, (float)y));
    }

    public void HandleMouseButton(Silk.NET.GLFW.WindowHandle* window, Silk.NET.GLFW.MouseButton button, Silk.NET.GLFW.InputAction action, Silk.NET.GLFW.KeyModifiers mods)
    {
        switch (action)
        {
            case Silk.NET.GLFW.InputAction.Press:
                OnButton?.Invoke(this, button.ToMouseButton(), true);
                break;

            case Silk.NET.GLFW.InputAction.Release:
                OnButton?.Invoke(this, button.ToMouseButton(), false);
                break;

            default:
                break;
        }
    }

#pragma warning restore IDE0060

    private const string name = "Mouse";

    private static readonly MouseButton[] buttons = new[]
    {
        MouseButton.Left,
        MouseButton.Right,
        MouseButton.Middle,
        MouseButton.Button1,
        MouseButton.Button2,
        MouseButton.Button3,
        MouseButton.Button4,
        MouseButton.Button5,
    };
}
