// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Silk.NET.Input;
using System.Numerics;

namespace Sekai.Framework.IO.HID;

/// <summary>
/// Snapshot of a input frame
/// </summary>
public interface InputSnapshot : IInputContext
{
    Vector2 MousePos { get; }

    float MouseWheelDelta { get; }
}
