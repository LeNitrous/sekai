// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

public abstract class JoystickEvent : ControllerEvent
{
    protected JoystickEvent(int index)
        : base(index)
    {
    }
}
