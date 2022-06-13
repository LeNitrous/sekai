// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Silk.NET.Input;

namespace Sekai.Framework.IO.HID;

/// <summary>
/// Snapshot of a input frame
/// </summary>
public interface IInputSnapshot : IInputContext
{
    Vector2 MousePos { get; }

    float MouseWheelDelta { get; }
}
