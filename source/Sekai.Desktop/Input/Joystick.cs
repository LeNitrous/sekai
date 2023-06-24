// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;

namespace Sekai.Desktop.Input;

internal sealed unsafe class Joystick : IJoystick, IGLFWController
{
    public string Name { get; }
    public Deadzone Deadzone { get; set; }
    public bool IsConnected { get; private set; } = true;
    public IReadOnlyList<Hat> Hats => hats;
    public IReadOnlyList<Axis> Axes => axes;
    public IReadOnlyList<Button> Buttons => buttons;

    public event Action<IJoystick, Hat>? OnHatMove;
    public event Action<IJoystick, Axis>? OnAxisMove;
    public event Action<IController, Button>? OnButton;

    private Hat[] hats = Array.Empty<Hat>();
    private Axis[] axes = Array.Empty<Axis>();
    private Button[] buttons = Array.Empty<Button>();
    private readonly int id;

    public Joystick(int id, string name)
    {
        Name = name;
        this.id = id;
    }

    public void Update(Silk.NET.GLFW.Glfw glfw)
    {
        if (glfw.JoystickPresent(id) && !glfw.JoystickIsGamepad(id))
        {
            pollHatState(glfw);
            pollAxisState(glfw);
            pollButtonState(glfw);
        }
        else
        {
            IsConnected = false;
        }
    }

    private void pollButtonState(Silk.NET.GLFW.Glfw glfw)
    {
        bool resize = false;
        byte* handles = glfw.GetJoystickButtons(id, out int count);

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

    private void pollAxisState(Silk.NET.GLFW.Glfw glfw)
    {
        bool resize = false;
        float* handles = glfw.GetJoystickAxes(id, out int count);

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

    private void pollHatState(Silk.NET.GLFW.Glfw glfw)
    {
        bool resize = false;
        var handles = glfw.GetJoystickHats(id, out int count);

        if (resize = axes.Length != count)
        {
            Array.Clear(hats);
            Array.Resize(ref hats, count);
        }

        for (int i = 0; i < count; i++)
        {
            var prev = hats[i];
            var next = new Hat(i, handles[i].ToHatPosition());

            if (resize || prev.Equals(next))
            {
                continue;
            }

            OnHatMove?.Invoke(this, hats[i] = next);
        }
    }
}
