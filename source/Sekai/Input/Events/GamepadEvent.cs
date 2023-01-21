// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

public abstract class GamepadEvent : ControllerEvent
{
    protected GamepadEvent(int index)
        : base(index)
    {
    }
}
