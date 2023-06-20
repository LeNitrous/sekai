// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Input;

namespace Sekai.Desktop.Input;

internal sealed unsafe class Gamepad : IGamepad, IGLFWController
{
    public string Name { get; }
    public bool IsConnected { get; private set; } = true;
    public IReadOnlyList<Thumbstick> Thumbsticks => thumbsticks;
    public IReadOnlyList<Trigger> Triggers => triggers;
    public IReadOnlyList<Button> Buttons => buttons;
    public IReadOnlyList<IMotor> Motors { get; } = Array.Empty<IMotor>();
    public Deadzone Deadzone { get; set; }

    public event Action<IGamepad, Trigger>? OnTriggerMove;
    public event Action<IGamepad, Thumbstick>? OnThumbstickMove;
    public event Action<IController, Button>? OnButton;

    private readonly Button[] buttons = new Button[15];
    private readonly Trigger[] triggers = new Trigger[2];
    private readonly Thumbstick[] thumbsticks = new Thumbstick[2];
    private readonly int id;

    public Gamepad(int id, string name)
    {
        Name = name;
        this.id = id;
    }

    public void Update(Silk.NET.GLFW.Glfw glfw)
    {
        if (!(glfw.JoystickPresent(id) && glfw.JoystickIsGamepad(id)))
        {
            IsConnected = false;
            return;
        }

        if (!glfw.GetGamepadState(id, out var state))
        {
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            var button = new Button(i, (ButtonName)i, state.Buttons[i] == 1);

            if (buttons[i].Equals(button))
            {
                continue;
            }

            OnButton?.Invoke(this, buttons[i] = button);
        }

        var thumbstick0 = new Thumbstick(0, Deadzone.Apply(state.Axes[0]), Deadzone.Apply(state.Axes[1]));

        if (!thumbsticks[0].Equals(thumbstick0))
        {
            OnThumbstickMove?.Invoke(this, thumbsticks[0] = thumbstick0);
        }

        var thumbstick1 = new Thumbstick(1, Deadzone.Apply(state.Axes[2]), Deadzone.Apply(state.Axes[3]));

        if (!thumbsticks[1].Equals(thumbstick1))
        {
            OnThumbstickMove?.Invoke(this, thumbsticks[1] = thumbstick1);
        }

        var trigger0 = new Trigger(0, Deadzone.Apply(state.Axes[4]));

        if (!triggers[0].Equals(trigger0))
        {
            OnTriggerMove?.Invoke(this, triggers[0] = trigger0);
        }

        var trigger1 = new Trigger(1, Deadzone.Apply(state.Axes[5]));

        if (!triggers[1].Equals(trigger0))
        {
            OnTriggerMove?.Invoke(this, triggers[1] = trigger1);
        }
    }
}
