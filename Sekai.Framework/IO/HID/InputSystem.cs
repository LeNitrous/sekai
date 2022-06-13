// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Numerics;
using Sekai.Framework.Services;
using Sekai.Framework.Systems;
using Silk.NET.Input;

namespace Sekai.Framework.IO.HID;

public class InputSystem : GameSystem, IUpdateable
{
    [Resolved]
    private Game game { get; }

    private readonly HashSet<KeyName> currentlyPressedKeys = new();

    private readonly HashSet<KeyName> newKeysThisFrame = new();

    private Vector2 prevMousePos;

    public Vector2? MousePosition
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

    public InputSnapshot? CurrentSnapshot { get; set; }

    public InputSystem(Game targetGame)
    {
        game = targetGame;
    }

    public void Update(double elapsed)
    {
        // TODO: Make a method here that gets a input snapshot per tick.
        game.Update(elapsed);
    }

    public void UpdateFrameInput(InputSnapshot snapshot)
    {
        CurrentSnapshot = snapshot;
        // ensure state is clear before we update.
        newKeysThisFrame.Clear();

        MouseDelta = CurrentSnapshot.MousePos - prevMousePos;
        prevMousePos = CurrentSnapshot.MousePos;

        // we can't do event-based polling, oh well....
        var kbs = snapshot.Keyboards;
        var mcs = snapshot.Mice;
        var jys = snapshot.Joysticks;

        foreach(var kb in kbs)
        {
            kb.KeyDown += keyDown;
            kb.KeyUp += keyUp;
        }

        foreach(var m in mcs)
        {
            // set the position on poll
            MousePosition = m.Position;
            m.MouseDown += keyDown;
            m.MouseUp += keyUp;
            m.MouseMove += onMouseMove;
            m.Scroll += onMouseScroll;
        }

        foreach(var j in jys)
        {
            j.ButtonUp += keyUp;
            j.ButtonDown += keyDown;
        }
    }

    public void OnNewSceneLoad()
    {
        ClearState();
    }

    public void ClearState()
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

    #region Keyboard handling code
    private void keyUp(IKeyboard kb, Key key, int id)
    {
        currentlyPressedKeys.Remove((KeyName)key);
        newKeysThisFrame.Remove((KeyName)key);
    }

    private void keyDown(IKeyboard kb, Key key, int id)
    {
        if (currentlyPressedKeys.Add((KeyName)key))
            newKeysThisFrame.Add((KeyName)key);
    }
    #endregion

    #region Mouse handling code
    private void onMouseMove(IMouse mouse, Vector2 pos)
    {
        // we'd love to set our snapshots here actually!
    }

    private void onMouseScroll(IMouse mouse, ScrollWheel wheel)
    {
        // TODO: calculate the delta of the scroll wheel here!
    }

    private void keyUp(IMouse mouse, MouseButton btn)
    {
        if (currentlyPressedKeys.Add((KeyName)btn))
            newKeysThisFrame.Add((KeyName)btn);
    }

    private void keyDown(IMouse mouse, MouseButton btn)
    {
        if (currentlyPressedKeys.Add((KeyName)btn))
            newKeysThisFrame.Add((KeyName)btn);
    }
    #endregion

    #region Joystick Handling code
    private void keyUp(IJoystick joystick, Button btn)
    {

    }

    private void keyDown(IJoystick joystick, Button btn)
    {

    }
    #endregion
}
