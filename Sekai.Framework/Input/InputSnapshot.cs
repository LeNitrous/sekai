// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.
// Derived from Veldrid.

using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Input;

namespace Sekai.Framework.Input;

public interface InputSnapshot
{
    // TODO: Make this listen from Key Events
    IReadOnlyList<IKeyboard> KeyEvents { get; }
    // TODO: Make this listen to Mouse Events
    IReadOnlyList<IMouse> MouseEvents { get; }
    IReadOnlyList<IKeyboard> KeyCharPresses { get; }
    bool IsMouseDown(MouseButton button);
    Vector2 MousePosition { get; }
    float WheelDelta { get; }
}
