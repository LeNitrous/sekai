// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Input.Devices.Pointers;

namespace Sekai.Input.States;

public sealed class MouseState : InputState<MouseButton>
{
    public Vector2 Position { get; set; }
    public ScrollWheel Scroll { get; set; }

    public override void Reset()
    {
        base.Reset();
        Scroll = new ScrollWheel();
        Position = Vector2.Zero;
    }
}
