// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Input;

namespace Sekai.GLFW;

internal sealed unsafe class GLFWMouse : IMouse
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

    public GLFWMouse(Silk.NET.GLFW.Glfw glfw, Silk.NET.GLFW.WindowHandle* window)
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
                OnButton?.Invoke(this, toMouseButton(button), true);
                break;

            case Silk.NET.GLFW.InputAction.Release:
                OnButton?.Invoke(this, toMouseButton(button), false);
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

    private static MouseButton toMouseButton(Silk.NET.GLFW.MouseButton button)
    {
        switch (button)
        {
            case Silk.NET.GLFW.MouseButton.Left:
                return MouseButton.Left;

            case Silk.NET.GLFW.MouseButton.Right:
                return MouseButton.Right;

            case Silk.NET.GLFW.MouseButton.Middle:
                return MouseButton.Middle;

            case Silk.NET.GLFW.MouseButton.Button4:
                return MouseButton.Button1;

            case Silk.NET.GLFW.MouseButton.Button5:
                return MouseButton.Button2;

            case Silk.NET.GLFW.MouseButton.Button6:
                return MouseButton.Button3;

            case Silk.NET.GLFW.MouseButton.Button7:
                return MouseButton.Button4;

            case Silk.NET.GLFW.MouseButton.Button8:
                return MouseButton.Button5;

            default:
                throw new ArgumentOutOfRangeException(nameof(button));
        }
    }
}
