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
    private readonly List<SDL2.SDL.SDL_Scancode> pressedKeys = new();

    public void HandleEvent(SDL2.SDL.SDL_KeyboardEvent keyboardEvent)
    {
        switch (keyboardEvent.type)
        {
            case SDL2.SDL.SDL_EventType.SDL_KEYDOWN:
                {
                    if (!contains(keyboardEvent.keysym.scancode))
                    {
                        pressedKeys.Add(keyboardEvent.keysym.scancode);
                        OnKeyDown?.Invoke(this, keyboardEvent.keysym.ToKey(), (int)keyboardEvent.keysym.scancode);
                    }
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

    public bool IsKeyPressed(Key key) => contains(key.ToScancode());

    private bool contains(SDL2.SDL.SDL_Scancode scancode)
    {
        for (int i = 0; i < pressedKeys.Count; i++)
        {
            if (pressedKeys[i] == scancode)
                return true;
        }

        return false;
    }
}
