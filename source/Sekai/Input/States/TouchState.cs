// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Input.Devices.Touch;

namespace Sekai.Input.States;

public sealed class TouchState : InputState<TouchSource>
{
    public readonly Vector2[] Positions = new Vector2[Enum.GetValues<TouchSource>().Length];

    public override void Reset()
    {
        base.Reset();
        Positions.AsSpan().Clear();
    }
}
