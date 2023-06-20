// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a rumble motor inside a gamepad.
/// </summary>
public interface IMotor
{
    /// <summary>
    /// The index of this motor.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// The motor's vibration intensitiy, between 0f and 1f.
    /// </summary>
    /// <remarks>
    /// Some backends may truncate this value if variable intensity is not supported.
    /// </remarks>
    float Speed { get; set; }
}
