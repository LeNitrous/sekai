// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;

namespace Sekai.GLFW;

internal sealed unsafe class GLFWJoystick : GLFWController, IJoystick
{
    public override bool IsConnected => Glfw.JoystickPresent(Index) && !Glfw.JoystickIsGamepad(Index);
    public IReadOnlyList<Hat> Hats => hats;
    public IReadOnlyList<Axis> Axes => axes;
    public IReadOnlyList<Button> Buttons => buttons;

    public event Action<IJoystick, Hat>? OnHatMove;
    public event Action<IJoystick, Axis>? OnAxisMove;
    public event Action<IController, Button>? OnButton;

    private Hat[] hats = Array.Empty<Hat>();
    private Axis[] axes = Array.Empty<Axis>();
    private Button[] buttons = Array.Empty<Button>();

    public GLFWJoystick(Silk.NET.GLFW.Glfw glfw, int index)
        : base(glfw, index)
    {
    }

    public override void Update()
    {
        if (IsConnected)
        {
            pollHatState();
            pollAxisState();
            pollButtonState();
        }
    }

    private void pollButtonState()
    {
        bool resize = false;
        byte* handles = Glfw.GetJoystickButtons(Index, out int count);

        if (resize = buttons.Length != count)
        {
            Array.Clear(buttons);
            Array.Resize(ref buttons, count);
        }

        for (int i = 0; i < count; i++)
        {
            var prev = buttons[i];
            var next = new Button(i, handles[i] == 1);

            if (resize || prev.Equals(next))
            {
                continue;
            }

            OnButton?.Invoke(this, buttons[i] = next);
        }
    }

    private void pollAxisState()
    {
        bool resize = false;
        float* handles = Glfw.GetJoystickAxes(Index, out int count);

        if (resize = axes.Length != count)
        {
            Array.Clear(axes);
            Array.Resize(ref axes, count);
        }

        for (int i = 0; i < count; i++)
        {
            var prev = axes[i];
            var next = new Axis(i, Deadzone.Apply(handles[i]));

            if (resize || prev.Equals(next))
            {
                continue;
            }

            OnAxisMove?.Invoke(this, axes[i] = next);
        }
    }

    private void pollHatState()
    {
        bool resize = false;
        var handles = Glfw.GetJoystickHats(Index, out int count);

        if (resize = axes.Length != count)
        {
            Array.Clear(hats);
            Array.Resize(ref hats, count);
        }

        for (int i = 0; i < count; i++)
        {
            var prev = hats[i];
            var next = new Hat(i, toHatPosition(handles[i]));

            if (resize || prev.Equals(next))
            {
                continue;
            }

            OnHatMove?.Invoke(this, hats[i] = next);
        }
    }

    private static HatPosition toHatPosition(Silk.NET.GLFW.JoystickHats hat)
    {
        switch (hat)
        {
            case Silk.NET.GLFW.JoystickHats.Centered:
                return HatPosition.Centered;

            case Silk.NET.GLFW.JoystickHats.Up:
                return HatPosition.Up;

            case Silk.NET.GLFW.JoystickHats.Right:
                return HatPosition.Right;

            case Silk.NET.GLFW.JoystickHats.RightUp:
                return HatPosition.UpRight;

            case Silk.NET.GLFW.JoystickHats.Down:
                return HatPosition.Down;

            case Silk.NET.GLFW.JoystickHats.RightDown:
                return HatPosition.DownRight;

            case Silk.NET.GLFW.JoystickHats.Left:
                return HatPosition.Left;

            case Silk.NET.GLFW.JoystickHats.LeftUp:
                return HatPosition.UpLeft;

            case Silk.NET.GLFW.JoystickHats.LeftDown:
                return HatPosition.DownLeft;

            default:
                throw new ArgumentOutOfRangeException(nameof(hat));
        }
    }
}
