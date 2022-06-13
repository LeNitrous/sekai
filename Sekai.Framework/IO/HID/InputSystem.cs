// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Services;
using Sekai.Framework.Systems;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sekai.Framework.IO.HID;

public class InputSystem : GameSystem, IUpdateable
{
    [Resolved]
    private Game game { get; }

    private readonly HashSet<KeyName> currentlyPressedKeys = new();

    private readonly HashSet<KeyName> newKeysThisFrame = new();

    private readonly List<Action<InputSystem>> cbs = new();

    private Vector2 prevMousePos;

    public Vector2 MousePosition
    {
        get => CurrentSnapshot.MousePos;
        set
        {
            // TODO: if we wanna set something here it has to be valid Vector2
            // coordinates relative to the window scale factor, similar to how
            // ge does it, but I don't know the best approach to it.
        }
    }

    public Vector2 MouseDelta { get; private set; }

    public InputSnapshot CurrentSnapshot { get; set; }

    public InputSystem()
    {
        // nothing to initialize here yet
        //
        // TODO: use the WindowFocusChanged handle to handle focus events
        // from the host.
    }

    public void RegisterCallback(Action<InputSystem> callback)
    {
        cbs.Add(callback);
    }

    public void Update(double elapsed)
    {
        // TODO: Make a method here that gets a input snapshot per tick.
        game.Update(elapsed);

        foreach (var cb in cbs)
            cb(this);
    }

    public void OnNewSceneLoad()
    {
        clearState();
    }

    public void WindowFocusChanged(bool isFocused)
    {
        // Regardless if we got back our focus, state must be cleared.
        // This prevents any ghosting of the input.
        if (isFocused)
            clearState();

        clearState();
    }

    private void clearState()
    {
        currentlyPressedKeys.Clear();
        newKeysThisFrame.Clear();
    }

    public bool GetKey(KeyName key)
    {
        return currentlyPressedKeys.Contains(key);
    }

    public bool GetKeyDown(KeyName key)
    {
        return newKeysThisFrame.Contains(key);
    }

    public void UpdateFrameInput(InputSnapshot snapshot)
    {
        CurrentSnapshot = snapshot;
        newKeysThisFrame.Clear();

        MouseDelta = CurrentSnapshot.MousePos - prevMousePos;
        prevMousePos = CurrentSnapshot.MousePos;

        var keyboardEvents = snapshot.KeyboardEvents;
        var mouseEvents = snapshot.MouseEvents;

        // FIXME: consolidate this on one loop maybe?
        foreach (var k in keyboardEvents)
        {
            // SDL_PRESSED is 0x01?
            // Silk uses Uint8 but according to
            // SDL_events.h, this:
            // #define SDL_RELEASED 0
            // #define SDL_PRESSED 1
            if (k.State == 0x01)
            {
                if (Enum.IsDefined(typeof(KeyName), k.Keysym.Scancode))
                    keyDown((KeyName)k.Keysym.Scancode);

                // We'd still register it, but it'll be unknown
                // HACK: jank ahoy! Debug the input code if you see this!
                keyDown(KeyName.Unknown);
            }
            else
            {
                if (Enum.IsDefined(typeof(KeyName), k.Keysym.Scancode))
                    keyUp((KeyName)k.Keysym.Scancode);

                // We'd still register it, but it'll be unknown
                // HACK: jank ahoy! Debug the input code if you see this!
                keyDown(KeyName.Unknown);
            }
        }

        foreach (var m in mouseEvents)
        {
            // same deal but nested
            // because lmao
            var mbes = m.MouseButtonEvents;

            foreach (var mbe in mbes)
            {
                if (mbe.State == 0x01)
                {
                    if (Enum.IsDefined(typeof(KeyName), mbe.Button))
                        keyDown((KeyName)mbe.Button);

                    // We'd still register it, but it'll be unknown
                    // HACK: jank ahoy! Debug the input code if you see this!
                    keyDown(KeyName.Unknown);
                }
                else
                {
                    if (Enum.IsDefined(typeof(KeyName), mbe.Button))
                        keyUp((KeyName)mbe.Button);

                    // We'd still register it, but it'll be unknown
                    // HACK: jank ahoy! Debug the input code if you see this!
                    keyUp(KeyName.Unknown);
                }
            }
        }
    }

    private void keyUp(KeyName key)
    {
        currentlyPressedKeys.Remove(key);
        newKeysThisFrame.Remove(key);
    }

    private void keyDown(KeyName key)
    {
        if (currentlyPressedKeys.Add(key))
            newKeysThisFrame.Add(key);
    }
}
