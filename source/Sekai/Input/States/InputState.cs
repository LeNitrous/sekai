// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input.States;

public abstract class InputState<T> : FrameworkObject
    where T : struct, Enum
{
    private readonly HashSet<T> buttons = new();

    public bool IsPressed(T button) => buttons.Contains(button);

    public void SetPressed(T button, bool state)
    {
        if (buttons.Contains(button) == state)
            return;

        if (state)
        {
            buttons.Add(button);
        }
        else
        {
            buttons.Remove(button);
        }
    }

    public virtual void Reset()
    {
        buttons.Clear();
    }
}
