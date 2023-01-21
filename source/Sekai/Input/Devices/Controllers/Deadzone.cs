// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// The gamepad/joystick's stick deadzone.
/// </summary>
public readonly struct Deadzone
{
    /// <summary>
    /// The deadzone size.
    /// </summary>
    public float Value { get; }

    /// <summary>
    /// The deadzone method.
    /// </summary>
    public DeadzoneMethod Method { get; }

    public Deadzone(float value, DeadzoneMethod method)
    {
        Value = value;
        Method = method;
    }

    public float Apply(float raw) => Method switch
    {
        DeadzoneMethod.Traditional => Math.Abs(raw) < Value ? 0 : raw,
        DeadzoneMethod.AdaptiveGradient => ((1 - Value) * raw) + (Value * Math.Sign(raw)),
        _ => throw new InvalidOperationException()
    };
}

public enum DeadzoneMethod
{
    Traditional,
    AdaptiveGradient,
}
