// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input.Devices.Keyboards;

/// <summary>
/// An interface representing a keyboard.
/// </summary>
public interface IKeyboard : IInputDevice
{
    /// <summary>
    /// A collection of keys the keyboard supports.
    /// </summary>
    IReadOnlyList<Key> Keys { get; }

    /// <summary>
    /// Shows the input method editor.
    /// </summary>
    void ShowIME();

    /// <summary>
    /// Hides the input method editor.
    /// </summary>
    void HideIME();

    /// <summary>
    /// Called when a <see cref="Key"/> has been pressed down.
    /// </summary>
    event Action<IKeyboard, Key>? OnKeyPressed;

    /// <summary>
    /// Called when a <see cref="Key"/> has been released.
    /// </summary>
    event Action<IKeyboard, Key>? OnKeyRelease;

    /// <summary>
    /// Called when the input method editor composition text has been submitted.
    /// </summary>
    event Action<IKeyboard, KeyboardIME>? OnCompositionSubmit;

    /// <summary>
    /// Called when the input method editor composition text has been changed.
    /// </summary>
    event Action<IKeyboard, KeyboardIME>? OnCompositionChange;
}
