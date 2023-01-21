// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Controllers;

namespace Sekai.Input.States;

public sealed class ControllerState : InputState<ControllerButton>
{
    public Hat[]? Hats { get; set; }
    public Axis[]? Axes { get; set; }
    public Trigger[]? Triggers { get; set; }
    public Thumbstick[]? Thumbsticks { get; set; }

    public override void Reset()
    {
        base.Reset();
        Hats = null;
        Axes = null;
        Triggers = null;
        Thumbsticks = null;
    }
}
