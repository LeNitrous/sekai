// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Input;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal class SDLKeyboard : FrameworkObject, IKeyboard
{
    public IReadOnlyList<Key> SupportedKeys { get; } = Enum.GetValues<Key>();
    public string Name { get; } = @"Keyboard";
    public int Index { get; } = 0;
    public bool IsConnected { get; } = true;
    public event Action<IKeyboard, Key, int?> OnKeyDown = null!;
    public event Action<IKeyboard, Key, int?> OnKeyUp = null!;
    private readonly List<Scancode> pressedKeys = new();

    private readonly SDLView view;

    public SDLKeyboard(SDLView view)
    {
        this.view = view;
        this.view.OnProcessEvent += onProcessEvent;
    }

    public bool IsKeyPressed(Key key) => contains(key.ToScancode());

    private bool contains(Scancode scancode)
    {
        for (int i = 0; i < pressedKeys.Count; i++)
        {
            if (pressedKeys[i] == scancode)
                return true;
        }

        return false;
    }

    private void handleEvent(KeyboardEvent keyboardEvent)
    {
        var type = (EventType)keyboardEvent.Type;

        switch (type)
        {
            case EventType.Keydown:
                {
                    if (!contains(keyboardEvent.Keysym.Scancode))
                    {
                        pressedKeys.Add(keyboardEvent.Keysym.Scancode);
                        OnKeyDown?.Invoke(this, keyboardEvent.Keysym.ToKey(), (int)keyboardEvent.Keysym.Scancode);
                    }
                    break;
                }

            case EventType.Keyup:
                {
                    if (pressedKeys.Remove(keyboardEvent.Keysym.Scancode))
                        OnKeyUp?.Invoke(this, keyboardEvent.Keysym.ToKey(), (int)keyboardEvent.Keysym.Scancode);
                    break;
                }

            default:
                break;
        }
    }

    private void onProcessEvent(Event evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Keyup:
            case EventType.Keydown:
                handleEvent(evt.Key);
                break;
        }
    }

    protected override void Destroy()
    {
        view.OnProcessEvent -= onProcessEvent;
    }
}
