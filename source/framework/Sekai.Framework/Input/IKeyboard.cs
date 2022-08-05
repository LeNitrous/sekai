// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Input;

/// <summary>
/// An interface representing a keyboard.
/// </summary>
public interface IKeyboard : IInputDevice
{
    /// <summary>
    /// List of all available keys for this keyboard.
    /// </summary>
    IReadOnlyList<Key> SupportedKeys { get; }

    /// <summary>
    /// Called when a key is pressed.
    /// </summary>
    event Action<IKeyboard, Key, int?> OnKeyDown;

    /// <summary>
    /// Called when a key is released.
    /// </summary>
    event Action<IKeyboard, Key, int?> OnKeyUp;

    /// <summary>
    /// Called when a character is received.
    /// </summary>
    event Action<IKeyboard, char> OnKeyChar;

    /// <summary>
    /// Returns whether a given key is pressed.
    /// </summary>
    bool IsKeyPressed(Key key);
}
