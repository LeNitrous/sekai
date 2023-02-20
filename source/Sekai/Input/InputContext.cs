// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Input.Devices.Controllers;
using Sekai.Input.Devices.Keyboards;
using Sekai.Input.Devices.MIDI;
using Sekai.Input.Devices.Pointers;
using Sekai.Input.Devices.Touch;
using Sekai.Input.Events;
using Sekai.Input.States;

namespace Sekai.Input;

public sealed class InputContext : DisposableObject
{
    /// <summary>
    /// A read-only collection of all available <see cref="InputSystem"/>.
    /// </summary>
    public IReadOnlyCollection<InputSystem> Systems => systems;

    /// <summary>
    /// Invoked when a device connection state has changed.
    /// </summary>
    public event Action<InputContext, IInputDevice, bool> OnDeviceConnectionChange;

    private const int max_controller_count = 8;

    private IPen? pen;
    private IMouse? mouse;
    private ITouch? touch;
    private IKeyboard? keyboard;
    private readonly IMIDIDevice?[] midiDevices = new IMIDIDevice[max_controller_count];
    private readonly IController?[] controllers = new IController[max_controller_count];

    private readonly PenState penState = new();
    private readonly MouseState mouseState = new();
    private readonly TouchState touchState = new();
    private readonly KeyboardState keyboardState = new();
    private readonly MIDIState[] midiState = Enumerable.Range(0, max_controller_count).Select(_ => new MIDIState()).ToArray();
    private readonly ControllerState[] controllerState = Enumerable.Range(0, max_controller_count).Select(_ => new ControllerState()).ToArray();

    private readonly HashSet<InputSystem> systems = new();
    private readonly ConcurrentQueue<DeviceEvent> events = new();

    internal InputContext(IEnumerable<InputSystem> systems)
    {
        foreach (var system in systems)
        {
            if (!this.systems.Add(system))
                continue;

            system.OnConnectionChanged += handleDeviceConnectionChange;
        }
    }

    /// <summary>
    /// Gets the position of the pen.
    /// </summary>
    public Vector2 GetPenPosition() => penState.Position;

    /// <summary>
    /// 
    /// </summary>
    public Vector2 GetMousePosition() => mouseState.Position;

    public Vector2 GetTouchPosition(TouchSource source) => touchState.Positions[(int)source];

    public bool IsPressed(PenButton button) => penState.IsPressed(button);

    public bool IsPressed(MouseButton button) => mouseState.IsPressed(button);

    public bool IsPressed(TouchSource source) => touchState.IsPressed(source);

    public bool IsPressed(Key key) => keyboardState.IsPressed(key);

    public bool IsPressed(Note note) => IsPressed(0, note);

    public bool IsPressed(int index, Note note) => midiState[index].IsPressed(note);

    public bool IsPressed(ControllerButton button) => IsPressed(0, button);

    public bool IsPressed(int index, ControllerButton button) => controllerState[index].IsPressed(button);

    public Hat GetControllerHat(int hatIndex = 0) => GetControllerHat(0, hatIndex);

    public Hat GetControllerHat(int controllerIndex, int hatIndex = 0) => controllerState[controllerIndex].Hats is null ? default : controllerState[controllerIndex].Hats![hatIndex];

    public Axis GetControllerAxis(int axisIndex = 0) => GetControllerAxis(0, axisIndex);

    public Axis GetControllerAxis(int controllerIndex, int axisIndex = 0) => controllerState[controllerIndex].Axes is null ? default : controllerState[controllerIndex].Axes![axisIndex];

    public Trigger GetControllerTrigger(int triggerIndex = 0) => GetControllerTrigger(0, triggerIndex);

    public Trigger GetControllerTrigger(int controllerIndex, int triggerIndex = 0) => controllerState[controllerIndex].Triggers is null ? default : controllerState[controllerIndex].Triggers![triggerIndex];

    public Thumbstick GetControllerThumbstick(int controllerIndex, int thumbstickIndex = 0) => controllerState[controllerIndex].Thumbsticks is null ? default : controllerState[controllerIndex].Thumbsticks![thumbstickIndex];

    internal void Update()
    {
        while (events.TryDequeue(out var evt))
        {
            switch (evt)
            {
                case PenMoveEvent pen:
                    penState.Position = pen.Position;
                    break;

                case PenButtonEvent pen:
                    penState.SetPressed(pen.Button, pen.IsPressed);
                    break;

                case MouseMoveEvent mouse:
                    mouseState.Position = mouse.Position;
                    break;

                case MouseButtonEvent mouse:
                    mouseState.SetPressed(mouse.Button, mouse.IsPressed);
                    break;

                case TouchMoveEvent touch:
                    touchState.Positions[(int)touch.Source] = touch.Position;
                    break;

                case TouchPressEvent touch:
                    touchState.SetPressed(touch.Source, touch.IsPressed);
                    break;

                case KeyboardKeyEvent keyboard:
                    keyboardState.SetPressed(keyboard.Key, keyboard.IsPressed);
                    break;

                case KeyboardCompositionSubmitEvent keyboard:
                    keyboardState.Text = keyboard.Text;
                    break;

                case KeyboardCompositionChangeEvent keyboard:
                    {
                        keyboardState.TextPending = keyboard.Text;
                        keyboardState.Position = keyboard.Position;
                        keyboardState.Length = keyboard.Length;
                        break;
                    }

                case MIDIConnectionEvent midi:
                    midiState[midi.Index].Reset();
                    break;

                case MIDINoteEvent midi:
                    midiState[midi.Index].SetPressed(midi.Note, midi.IsPressed);
                    break;

                case ControllerConnectionEvent controller when controller.IsConnected:
                    {
                        var state = controllerState[controller.Index];

                        if (controller.HatCount > 0)
                            state.Hats = new Hat[controller.HatCount];

                        if (controller.AxisCount > 0)
                            state.Axes = new Axis[controller.AxisCount];

                        if (controller.TriggerCount > 0)
                            state.Triggers = new Trigger[controller.TriggerCount];

                        if (controller.ThumbstickCount > 0)
                            state.Thumbsticks = new Thumbstick[controller.ThumbstickCount];

                        break;
                    }

                case ControllerConnectionEvent controller when !controller.IsConnected:
                    controllerState[controller.Index].Reset();
                    break;

                case ControllerButtonEvent controller:
                    controllerState[controller.Index].SetPressed(controller.Button, controller.IsPressed);
                    break;

                case JoystickHatEvent joy:
                    controllerState[joy.Index].Hats![joy.Hat.Index] = joy.Hat;
                    break;

                case JoystickAxisEvent joy:
                    controllerState[joy.Index].Axes![joy.Axis.Index] = joy.Axis;
                    break;

                case GamepadTriggerEvent pad:
                    controllerState[pad.Index].Triggers![pad.Trigger.Index] = pad.Trigger;
                    break;

                case GamepadThumbstickEvent pad:
                    controllerState[pad.Index].Thumbsticks![pad.Thumbstick.Index] = pad.Thumbstick;
                    break;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var system in systems)
        {
            if (!systems.Remove(system))
                continue;

            system.OnConnectionChanged -= handleDeviceConnectionChange;
        }

        systems.Clear();
    }

    private void handleDeviceConnectionChange(IInputDevice device, bool state)
    {
        if (state)
        {
            attach(device);
        }
        else
        {
            detach(device);
        }
    }

    private bool attach(IInputDevice device)
    {
        OnDeviceConnectionChange?.Invoke(this, device, true);

        switch (device)
        {
            case IPen pen:
                return attach(pen);

            case IMouse mouse:
                return attach(mouse);

            case ITouch touch:
                return attach(touch);

            case IKeyboard keyboard:
                return attach(keyboard);

            case IMIDIDevice midiDevice:
                return attach(midiDevice);

            case IController controller:
                return attach(controller);

            default:
                throw new NotSupportedException(@"Device is not supported.");
        }
    }

    private bool detach(IInputDevice device)
    {
        OnDeviceConnectionChange?.Invoke(this, device, false);

        switch (device)
        {
            case IPen pen:
                return detach(pen);

            case IMouse mouse:
                return detach(mouse);

            case ITouch touch:
                return detach(touch);

            case IKeyboard keyboard:
                return detach(keyboard);

            case IMIDIDevice midiDevice:
                return detach(midiDevice);

            case IController controller:
                return detach(controller);

            default:
                throw new NotSupportedException(@"Device is not supported.");
        }
    }

    private bool attach(IMouse mouse)
    {
        if (this.mouse is not null)
            return false;

        this.mouse = mouse;
        this.mouse.OnMove += handleMouseMove;
        this.mouse.OnScroll += handleMouseScroll;
        this.mouse.OnButtonPressed += handleMouseButtonPressed;
        this.mouse.OnButtonRelease += handleMouseButtonRelease;

        return true;
    }

    private bool detach(IMouse mouse)
    {
        if (mouse != this.mouse)
            return false;

        this.mouse.OnMove -= handleMouseMove;
        this.mouse.OnScroll -= handleMouseScroll;
        this.mouse.OnButtonPressed -= handleMouseButtonPressed;
        this.mouse.OnButtonRelease -= handleMouseButtonRelease;
        this.mouse = null;

        return true;
    }

    private void handleMouseMove(IPointer pointer, Vector2 position)
        => events.Enqueue(new MouseMoveEvent(position));

    private void handleMouseScroll(IMouse mouse, ScrollWheel scroll)
        => events.Enqueue(new MouseScrollEvent(scroll));

    private void handleMouseButtonPressed(IMouse mouse, MouseButton button)
        => events.Enqueue(new MouseButtonEvent(button, true));

    private void handleMouseButtonRelease(IMouse mouse, MouseButton button)
        => events.Enqueue(new MouseButtonEvent(button, false));

    private bool attach(IKeyboard keyboard)
    {
        if (this.keyboard is not null)
            return false;

        this.keyboard = keyboard;
        this.keyboard.OnKeyPressed += handleKeyboardKeyPressed;
        this.keyboard.OnKeyRelease += handleKeyboardKeyRelease;
        this.keyboard.OnCompositionSubmit += handleKeyboardCompositionSubmit;
        this.keyboard.OnCompositionChange += handleKeyboardCompositionChange;

        return true;
    }

    private bool detach(IKeyboard keyboard)
    {
        if (keyboard != this.keyboard)
            return false;

        this.keyboard.OnKeyPressed -= handleKeyboardKeyPressed;
        this.keyboard.OnKeyRelease -= handleKeyboardKeyRelease;
        this.keyboard.OnCompositionSubmit -= handleKeyboardCompositionSubmit;
        this.keyboard.OnCompositionChange -= handleKeyboardCompositionChange;
        this.keyboard = null;

        return true;
    }

    private void handleKeyboardKeyPressed(IKeyboard keyboard, Key key)
        => events.Enqueue(new KeyboardKeyEvent(key, true));

    private void handleKeyboardKeyRelease(IKeyboard keyboard, Key key)
        => events.Enqueue(new KeyboardKeyEvent(key, false));

    private void handleKeyboardCompositionSubmit(IKeyboard keyboard, KeyboardIME ime)
        => events.Enqueue(new KeyboardCompositionSubmitEvent(ime.Text));

    private void handleKeyboardCompositionChange(IKeyboard keyboard, KeyboardIME ime)
        => events.Enqueue(new KeyboardCompositionChangeEvent(ime.Start, ime.Length, ime.Text));

    private bool attach(IPen pen)
    {
        if (this.pen is not null)
            return false;

        this.pen = pen;
        this.pen.OnMove += handlePenMove;
        this.pen.OnButtonPressed += handlePenButtonPressed;
        this.pen.OnButtonRelease += handlePenButtonRelease;

        return true;
    }

    private bool detach(IPen pen)
    {
        if (this.pen != pen)
            return false;

        this.pen.OnMove -= handlePenMove;
        this.pen.OnButtonPressed -= handlePenButtonPressed;
        this.pen.OnButtonRelease -= handlePenButtonRelease;
        this.pen = null;

        return true;
    }

    private void handlePenMove(IPointer pointer, Vector2 position)
        => events.Enqueue(new PenMoveEvent(position));

    private void handlePenButtonPressed(IPen pen, PenButton button)
        => events.Enqueue(new PenButtonEvent(button, true));

    private void handlePenButtonRelease(IPen pen, PenButton button)
        => events.Enqueue(new PenButtonEvent(button, false));

    private bool attach(ITouch touch)
    {
        if (this.touch is not null)
            return false;

        this.touch = touch;
        this.touch.OnMove += handleTouchMove;
        this.touch.OnPressed += handleTouchPressed;
        this.touch.OnRelease += handleTouchRelease;

        return true;
    }

    private bool detach(ITouch touch)
    {
        if (this.touch != touch)
            return false;

        this.touch.OnMove -= handleTouchMove;
        this.touch.OnPressed -= handleTouchPressed;
        this.touch.OnRelease -= handleTouchRelease;
        this.touch = null;

        return true;
    }

    private void handleTouchMove(ITouch touch, TouchData data)
        => events.Enqueue(new TouchMoveEvent(data.Source, data.Position));

    private void handleTouchPressed(ITouch touch, TouchData data)
        => events.Enqueue(new TouchPressEvent(data.Source, true));

    private void handleTouchRelease(ITouch touch, TouchData data)
        => events.Enqueue(new TouchPressEvent(data.Source, false));

    private bool attach(IController controller)
    {
        if (Array.IndexOf(controllers, controller) >= 0)
            return false;

        bool hasAttached = false;

        for (int i = 0; i < max_controller_count; i++)
        {
            if (controllers[i] is null)
            {
                hasAttached = true;

                controllers[i] = controller;
                controller.OnButtonPressed += handleControllerButtonPressed;
                controller.OnButtonRelease += handleControllerButtonRelease;

                switch (controller)
                {
                    case IJoystick joy:
                        {
                            joy.OnHatMove += handleJoystickHatMove;
                            joy.OnAxisMove += handleJoystickAxisMove;
                            handleControllerConnection(joy, i, true);
                            break;
                        }

                    case IGamepad pad:
                        {
                            pad.OnTriggerMove += handleGamepadTriggerMove;
                            pad.OnThumbstickMove += handleGamepadThumbstickMove;
                            handleControllerConnection(pad, i, true);
                            break;
                        }
                }

                break;
            }
            else
            {
                continue;
            }
        }

        return hasAttached;
    }

    private bool detach(IController controller)
    {
        int index = Array.IndexOf(controllers, controller);

        if (index < 0)
            return false;

        controller.OnButtonPressed -= handleControllerButtonPressed;
        controller.OnButtonRelease -= handleControllerButtonRelease;

        switch (controller)
        {
            case IJoystick joy:
                {
                    joy.OnHatMove -= handleJoystickHatMove;
                    joy.OnAxisMove -= handleJoystickAxisMove;
                    handleControllerConnection(joy, index, false);
                    break;
                }

            case IGamepad pad:
                {
                    pad.OnTriggerMove -= handleGamepadTriggerMove;
                    pad.OnThumbstickMove -= handleGamepadThumbstickMove;
                    handleControllerConnection(pad, index, false);
                    break;
                }
        }

        controllers[index] = null;

        return true;
    }

    private void handleControllerConnection(IJoystick joystick, int index, bool state)
        => events.Enqueue(new ControllerConnectionEvent(index, 0, 0, joystick.Hats.Count, joystick.Axes.Count, state));

    private void handleControllerConnection(IGamepad gamepad, int index, bool state)
        => events.Enqueue(new ControllerConnectionEvent(index, gamepad.Triggers.Count, gamepad.Thumbsticks.Count, 0, 0, state));

    private void handleControllerButtonPressed(IController controller, ControllerButton button)
        => events.Enqueue(new ControllerButtonEvent(Array.IndexOf(controllers, controller), button, true));

    private void handleControllerButtonRelease(IController controller, ControllerButton button)
        => events.Enqueue(new ControllerButtonEvent(Array.IndexOf(controllers, controller), button, false));

    private void handleJoystickAxisMove(IJoystick joystick, Axis axis)
        => events.Enqueue(new JoystickAxisEvent(Array.IndexOf(controllers, joystick), axis));

    private void handleJoystickHatMove(IJoystick joystick, Hat hat)
        => events.Enqueue(new JoystickHatEvent(Array.IndexOf(controllers, joystick), hat));

    private void handleGamepadThumbstickMove(IGamepad gamepad, Thumbstick thumbstick)
        => events.Enqueue(new GamepadThumbstickEvent(Array.IndexOf(controllers, gamepad), thumbstick));

    private void handleGamepadTriggerMove(IGamepad gamepad, Trigger trigger)
        => events.Enqueue(new GamepadTriggerEvent(Array.IndexOf(controllers, gamepad), trigger));

    private bool attach(IMIDIDevice device)
    {
        if (Array.IndexOf(midiDevices, device) >= 0)
            return false;

        bool hasAttached = false;

        for (int i = 0; i < max_controller_count; i++)
        {
            if (midiDevices[i] is null)
            {
                hasAttached = true;

                midiDevices[i] = device;
                device.OnNotePressed += handleMIDINotePressed;
                device.OnNoteRelease += handleMIDINoteRelease;
                handleMIDIConnection(i, true);

                break;
            }
            else
            {
                continue;
            }
        }

        return hasAttached;
    }

    private bool detach(IMIDIDevice device)
    {
        int index = Array.IndexOf(midiDevices, device);

        if (index < 0)
            return false;

        device.OnNotePressed -= handleMIDINotePressed;
        device.OnNoteRelease -= handleMIDINoteRelease;

        midiDevices[index] = null;
        handleMIDIConnection(index, false);

        return true;
    }

    private void handleMIDIConnection(int index, bool state)
        => events.Enqueue(new MIDIConnectionEvent(index, state));

    private void handleMIDINotePressed(IMIDIDevice device, Note note)
        => events.Enqueue(new MIDINoteEvent(Array.IndexOf(midiDevices, device), note, true));

    private void handleMIDINoteRelease(IMIDIDevice device, Note note)
        => events.Enqueue(new MIDINoteEvent(Array.IndexOf(midiDevices, device), note, false));
}
