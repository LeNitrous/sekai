// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Input;

namespace Sekai;

/// <summary>
/// Handles the state of input.
/// </summary>
public sealed class InputState : IDisposable
{
    /// <summary>
    /// The underlying input context.
    /// </summary>
    public IInputSource Context { get; }

    /// <summary>
    /// The mouse position.
    /// </summary>
    public Vector2 MousePosition { get; private set; }

    /// <summary>
    /// The mouse scroll.
    /// </summary>
    public Vector2 MouseScroll { get; private set; }

    private bool isDisposed;
    private Int128 keyboardKeyState;
    private MouseButton mouseButtonState;
    private IMouse? primaryMouse;
    private IKeyboard? primaryKeyboard;

    internal InputState(IInputSource context)
    {
        Context = context;

        foreach (var device in Context.Devices)
        {
            handleDeviceConnect(device);
        }

        Context.ConnectionChanged += handleDeviceConnect;
    }

    /// <summary>
    /// Gets whether a given <see cref="MouseButton"/> is pressed.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns><see langword="true"/> if the button is pressed. Otherwise <see langword="false"/>.</returns>
    public bool IsPressed(MouseButton button)
    {
        return (mouseButtonState & button) != 0;
    }

    /// <summary>
    /// Gets whether a given <see cref="Key"/> is pressed.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the key is pressed. Otherwise <see langword="false"/>.</returns>
    public bool IsPressed(Key key)
    {
        return ((keyboardKeyState >> (int)key) & 1) != 0;
    }

    private void handleDeviceConnect(IInputDevice device, bool connected)
    {
        if (connected)
        {
            handleDeviceConnect(device);
        }
        else
        {
            handleDeviceRemoval(device);
        }
    }

    private void handleDeviceConnect(IInputDevice device)
    {
        switch (device)
        {
            case IMouse mouse when primaryMouse is null:
                mouse.OnMove += handleMouseMotion;
                mouse.OnButton += handleMouseButton;
                mouse.OnScroll += handleMouseScroll;
                primaryMouse = mouse;
                break;

            case IKeyboard keyboard when primaryKeyboard is null:
                keyboard.OnKey += handleKeyboardKey;
                primaryKeyboard = keyboard;
                break;
        }
    }

    private void handleDeviceRemoval(IInputDevice device)
    {
        switch (device)
        {
            case IMouse mouse when ReferenceEquals(primaryMouse, mouse):
                mouse.OnMove -= handleMouseMotion;
                mouse.OnButton -= handleMouseButton;
                mouse.OnScroll -= handleMouseScroll;
                primaryMouse = null;
                break;

            case IKeyboard keyboard when ReferenceEquals(primaryKeyboard, keyboard):
                keyboard.OnKey -= handleKeyboardKey;
                primaryKeyboard = null;
                break;
        }
    }

    private void handleMouseMotion(IPointer pointer, Vector2 position)
    {
        MousePosition = position;
    }

    private void handleMouseButton(IMouse mouse, MouseButton button, bool pressed)
    {
        if (pressed)
        {
            mouseButtonState |= button;
        }
        else
        {
            mouseButtonState &= ~button;
        }
    }

    private void handleMouseScroll(IMouse mouse, ScrollWheel wheel)
    {
        MouseScroll = new(wheel.X, wheel.Y);
    }

    private void handleKeyboardKey(IKeyboard keyboard, Key key, bool pressed)
    {
        if (pressed)
        {
            keyboardKeyState |= (Int128)1 << (int)key;
        }
        else
        {
            keyboardKeyState &= ~((Int128)1 << (int)key);
        }
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        Context.ConnectionChanged -= handleDeviceConnect;

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
