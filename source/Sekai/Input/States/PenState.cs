// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Input.Devices.Pointers;

namespace Sekai.Input.States;

public sealed class PenState : InputState<PenButton>
{
    public Vector2 Position { get; set; }
}
