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
#pragma warning disable CS8602
        get => CurrentSnapshot.MousePos;
#pragma warning restore CS8602
        set
        {
            // TODO: if we wanna set something here it has to be valid Vector2
            // coordinates relative to the window scale factor, similar to how
            // ge does it, but I don't know the best approach to it.
        }
    }

    public Vector2 MouseDelta { get; private set; }

    public IInputSnapshot? CurrentSnapshot { get; set; }

    public InputSystem(Game targetGame)
    {
        game = targetGame;
    }

    public void Update(double elapsed)
    {
        //TODO: CurrentSnapshot is just a placeholder here
        // We want that snapshot to be made at the game level
        // so we can process it here at UpdateFrameInput.
        UpdateFrameInput(CurrentSnapshot);
        game.Update(elapsed);
    }

    public void UpdateFrameInput(IInputSnapshot snapshot)
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

        foreach (var kb in kbs)
        {
            kb.KeyDown += keyDown;
            kb.KeyUp += keyUp;
        }

        foreach (var m in mcs)
        {
            // set the position on poll
            MousePosition = m.Position;
            m.MouseDown += keyDown;
            m.MouseUp += keyUp;
            m.MouseMove += onMouseMove;
            m.Scroll += onMouseScroll;
        }

        foreach (var j in jys)
        {
            j.ButtonUp += keyUp;
            j.ButtonDown += keyDown;
            j.AxisMoved += onAxisMoved;
            j.HatMoved += onHatMoved;
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
        // TODO: Set mouse snapshot here for position.
    }

    private void onMouseScroll(IMouse mouse, ScrollWheel wheel)
    {
        // TODO: Set mouse snapshot delta here.
    }

    private void keyUp(IMouse mouse, MouseButton btn)
    {
        currentlyPressedKeys.Remove((KeyName)btn);
        newKeysThisFrame.Remove((KeyName)btn);
    }

    private void keyDown(IMouse mouse, MouseButton btn)
    {
        if (currentlyPressedKeys.Add((KeyName)btn))
            newKeysThisFrame.Add((KeyName)btn);
    }
    #endregion

    #region Joystick Handling code
    private void onAxisMoved(IJoystick joystick, Axis axis)
    {

    }

    private void onHatMoved(IJoystick joystick, Hat hat)
    {

    }

    private void keyUp(IJoystick joystick, Button btn)
    {
        // Button is a struct unlike the rest, so we will
        // have to parse it's ID then map this to KeyName enum.
        currentlyPressedKeys.Remove((KeyName)btn.Index);
        newKeysThisFrame.Remove((KeyName)btn.Index);
    }

    private void keyDown(IJoystick joystick, Button btn)
    {
        if (currentlyPressedKeys.Add((KeyName)btn.Index))
            newKeysThisFrame.Add((KeyName)btn.Index);
    }
    #endregion
}
