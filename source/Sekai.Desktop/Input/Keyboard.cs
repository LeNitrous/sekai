// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;

namespace Sekai.Desktop.Input;

internal sealed unsafe class Keyboard : IKeyboard
{
    public IReadOnlyList<Key> Keys => keys;
    public string Name => name;
    public bool IsConnected => true;
    public event Action<IKeyboard, Key, bool>? OnKey;

#pragma warning disable IDE0060

    public void HandleKeyboardKey(Silk.NET.GLFW.WindowHandle* window, Silk.NET.GLFW.Keys key, int scancode, Silk.NET.GLFW.InputAction action, Silk.NET.GLFW.KeyModifiers mods)
    {
        switch (action)
        {
            case Silk.NET.GLFW.InputAction.Press:
                OnKey?.Invoke(this, key.ToKey(), true);
                break;

            case Silk.NET.GLFW.InputAction.Release:
                OnKey?.Invoke(this, key.ToKey(), false);
                break;

            default:
                break;
        }
    }

#pragma warning restore IDE0060

    private const string name = "Keyboard";

    private static readonly Key[] keys = Enum.GetValues<Key>();
}
