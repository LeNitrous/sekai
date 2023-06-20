// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a keyboard.
/// </summary>
public interface IKeyboard : IInputDevice
{
    /// <summary>
    /// The available keys the keyboard has.
    /// </summary>
    IReadOnlyList<Key> Keys { get; }

    /// <summary>
    /// Called when a key event has occured.
    /// </summary>
    event Action<IKeyboard, Key, bool>? OnKey;
}
