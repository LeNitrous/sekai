// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sekai.Input.Devices.Keyboards;
using Silk.NET.SDL;

namespace Sekai.SDL;

internal class SDLKeyboard : FrameworkObject, IKeyboard
{
    public string Name { get; } = @"Keyboard";
    public IReadOnlyList<Key> Keys { get; } = Enum.GetValues<Key>();

    public event Action<IKeyboard, Key>? OnKeyPressed;
    public event Action<IKeyboard, Key>? OnKeyRelease;
    public event Action<IKeyboard, KeyboardIME>? OnCompositionSubmit;
    public event Action<IKeyboard, KeyboardIME>? OnCompositionChange;

    private readonly SDLSurface surface;

    public SDLKeyboard(SDLSurface surface)
    {
        this.surface = surface;
        this.surface.OnProcessEvent += onProcessEvent;
    }

    private void handleKeyEvent(KeyboardEvent keyboardEvent)
    {
        var type = (EventType)keyboardEvent.Type;

        switch (type)
        {
            case EventType.Keydown:
                OnKeyPressed?.Invoke(this, keyboardEvent.Keysym.ToKey());
                break;

            case EventType.Keyup:
                OnKeyRelease?.Invoke(this, keyboardEvent.Keysym.ToKey());
                break;

            default:
                break;
        }
    }

    private unsafe void handleTextInputEvent(TextInputEvent textEvent)
    {
        string text = Marshal.PtrToStringUTF8((nint)textEvent.Text) ?? string.Empty;
        OnCompositionSubmit?.Invoke(this, new(text, 0, text.Length));
    }

    private unsafe void handleTextEditingEvent(TextEditingEvent textEvent)
    {
        string text = Marshal.PtrToStringUTF8((nint)textEvent.Text) ?? string.Empty;
        OnCompositionChange?.Invoke(this, new(text, textEvent.Start, textEvent.Length));
    }

    private void onProcessEvent(Event evt)
    {
        var type = (EventType)evt.Type;

        switch (type)
        {
            case EventType.Keyup:
            case EventType.Keydown:
                handleKeyEvent(evt.Key);
                break;

            case EventType.Textinput:
                handleTextInputEvent(evt.Text);
                break;

            case EventType.Textediting:
                handleTextEditingEvent(evt.Edit);
                break;
        }
    }

    public void ShowIME()
    {
        surface.Invoke(surface.Sdl.StartTextInput);
    }

    public void HideIME()
    {
        surface.Invoke(surface.Sdl.StopTextInput);
    }

    protected override void Destroy()
    {
        surface.OnProcessEvent -= onProcessEvent;
    }
}
