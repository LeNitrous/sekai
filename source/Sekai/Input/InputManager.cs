// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Input;

public sealed class InputManager
{
    /// <summary>
    /// Get the current mouse position.
    /// </summary>
    public Point MousePosition { get; private set; }

    private Int128 keys;
    private MouseButton mouseButtons;
    private readonly IInputSource source;

    internal InputManager(IInputSource source)
    {
        this.source = source;
    }

    internal void Update()
    {
        foreach (var data in source.PumpEvents())
        {
            switch (data.Kind)
            {
                case EventKind.MouseMotion:
                    MousePosition = data.MouseMotion.Position;
                    break;

                case EventKind.MouseButton when data.MouseButton.Pressed:
                    mouseButtons |= data.MouseButton.Button;
                    break;

                case EventKind.MouseButton when !data.MouseButton.Pressed:
                    mouseButtons &= ~data.MouseButton.Button;
                    break;

                case EventKind.Keyboard when data.Keyboard.Pressed:
                    keys |= new Int128(0, 1) << (int)data.Keyboard.Key;
                    break;

                case EventKind.Keyboard when data.Keyboard.Pressed:
                    keys &= ~(new Int128(0, 1) << (int)data.Keyboard.Key);
                    break;
            }
        }
    }

    /// <summary>
    /// Gets whether a <see cref="MouseButton"/> is down or not.
    /// </summary>
    /// <param name="button">The button to check.</param>
    public bool IsDown(MouseButton button) => (button & mouseButtons) == 0;

    /// <summary>
    /// Gets whether a <see cref="Key"/> is down or not.
    /// </summary>
    /// <param name="key">The key to check.</param>
    public bool IsDown(Key key) => (keys & (new Int128(0, 1) << (int)key)) == 0;
}
