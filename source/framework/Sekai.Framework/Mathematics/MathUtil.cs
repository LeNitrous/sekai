// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Mathematics;

public static class MathUtil
{
    public const float ZeroTolerance = 1e-6f;

    /// <summary>
    /// Convert degrees to radians.
    /// </summary>
    public static float DegreesToRadians(float degrees)
    {
        return degrees * MathF.PI / 180.0f;
    }

    /// <summary>
    /// Convert radians to degrees.
    /// </summary>
    public static float RadiansToDegrees(float radians)
    {
        return radians * 180.0f / MathF.PI;
    }

    /// <summary>
    /// Wraps a given value between the minimum and maximum.
    /// </summary>
    public static float Wrap(float value, float min, float max)
    {
        return value < min
            ? max - ((min - value) % (max - min))
            : min + ((value - min) % (max - min));
    }
}
