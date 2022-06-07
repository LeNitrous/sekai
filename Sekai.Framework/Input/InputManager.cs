// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.
using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Sekai.Framework.Input;

public class InputSystem : FrameworkComponent
{
    private readonly IWindow window;

    private readonly HashSet<Key> currentlyPressedKeys = new HashSet<Key>();
    private readonly HashSet<Key> newKeysThisFrame = new HashSet<Key>();

    private readonly HashSet<MouseButton> currentlyPressedMouseButtons = new HashSet<MouseButton>();
    private readonly HashSet<MouseButton> newMouseButtonsThisFrame = new HashSet<MouseButton>();

    private readonly List<Action<InputSystem>> callbacks = new List<Action<InputSystem>>();
    private readonly List<Action<InputSystem>> actions = new List<Action<InputSystem>>();

    private Vector2 previousMousePos;

    public Vector2 MousePosition { get; set; }

    public Vector2 MouseDelta { get; set; }

    public InputSnapshot CurrentSnapshot { get; private set; }

    public InputSystem(IWindow window)
    {
        // TODO: you might want to register WindowGainedFocus and WindowLostFocus here.
        this.window = window;
    }

    /// <summary>
    /// Registers an anonymous callback which is invoked on every update of the InputSystem.
    /// </summary>
    public void RegisterCallback(Action<InputSystem> cb)
    {
        callbacks.Add(cb);
    }

    public void Update()
    {
        // TODO: in ge, this should update the input frame but @LeNitrous made an input thread, maybe delegate this to there instead?
        // See: https://github.com/mellinoe/ge/blob/master/src/Engine/InputSystem.cs#L74
        foreach(var cb in callbacks)
        {
            cb(this);
        }
    }

    public void WindowFocusLost()
    {
        ClearState();
    }

    public void WindowFocusGained()
    {
        ClearState();
    }

    protected void OnNewScene()
    {
        ClearState();
    }

    private void ClearState()
    {
        currentlyPressedKeys.Clear();
        newKeysThisFrame.Clear();
        currentlyPressedMouseButtons.Clear();
        newMouseButtonsThisFrame.Clear();
    }
}
