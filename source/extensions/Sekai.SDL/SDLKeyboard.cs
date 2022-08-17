// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Input;

namespace Sekai.SDL;

internal class SDLKeyboard : IKeyboard
{
    public IReadOnlyList<Key> SupportedKeys { get; } = Enum.GetValues<Key>();
    public string Name { get; } = @"Keyboard";
    public int Index { get; } = 0;
    public bool IsConnected { get; } = true;
    public event Action<IKeyboard, Key, int?> OnKeyDown = null!;
    public event Action<IKeyboard, Key, int?> OnKeyUp = null!;
    private readonly HashSet<SDL2.SDL.SDL_Scancode> pressedKeys = new();

    public void HandleEvent(SDL2.SDL.SDL_KeyboardEvent keyboardEvent)
    {
        switch (keyboardEvent.type)
        {
            case SDL2.SDL.SDL_EventType.SDL_KEYDOWN:
                {
                    if (pressedKeys.Add(keyboardEvent.keysym.scancode))
                        OnKeyDown?.Invoke(this, keyboardEvent.keysym.ToKey(), (int)keyboardEvent.keysym.scancode);
                    break;
                }

            case SDL2.SDL.SDL_EventType.SDL_KEYUP:
                {
                    if (pressedKeys.Remove(keyboardEvent.keysym.scancode))
                        OnKeyUp?.Invoke(this, keyboardEvent.keysym.ToKey(), (int)keyboardEvent.keysym.scancode);
                    break;
                }

            default:
                break;
        }
    }

    public bool IsKeyPressed(Key key) => pressedKeys.Contains(key.ToScancode());
}
