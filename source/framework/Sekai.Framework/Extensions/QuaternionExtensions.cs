// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Extensions;

public static class QuaternionExtensions
{
    /// <summary>
    /// Convert this quaternion to euler angles.
    /// </summary>
    public static void ToEulerAngles(this Quaternion q, out Vector3 euler)
    {
        float sinR = 2 * ((q.W * q.X) + (q.Y * q.Z));
        float cosR = 1 - (2 * ((q.X * q.X) + (q.Y * q.Y)));
        float roll = MathF.Atan2(sinR, cosR);

        float sinP = 2 * ((q.W * q.Y) - (q.Z * q.X));
        float pitch = MathF.Abs(sinP) >= 1
            ? MathF.CopySign((float)Math.PI / 2, sinP)
            : MathF.Asin(sinP);

        float sinY = 2 * ((q.W * q.Z) + (q.X * q.Y));
        float cosY = 1 - (2 * ((q.Y * q.Y) + (q.Z * q.Z)));
        float yaw = MathF.Atan2(sinY, cosY);

        euler.X = yaw;
        euler.Y = pitch;
        euler.Z = roll;
    }

    /// <summary>
    /// Gets this quaternion's forward vector.
    /// </summary>
    public static void GetForward(this Quaternion q, out Vector3 vector)
    {
        vector.X = 2 * ((q.X * q.Z) + (q.W * q.Y));
        vector.Y = 2 * ((q.Y * q.Z) - (q.W * q.X));
        vector.Z = 1 - (2 * ((q.X * q.X) + (q.Y * q.Y)));
    }

    /// <summary>
    /// Gets this quaternion's up vector.
    /// </summary>
    public static void GetUp(this Quaternion q, out Vector3 vector)
    {
        vector.X = 2 * ((q.X * q.Y) - (q.W * q.Z));
        vector.Y = 1 - (2 * ((q.X * q.X) + (q.Z * q.Z)));
        vector.Z = 2 * ((q.Y * q.Z) + (q.W * q.X));
    }

    /// <summary>
    /// Gets this quaternion's right vector.
    /// </summary>
    public static void GetRight(this Quaternion q, out Vector3 vector)
    {
        vector.X = 1 - (2 * ((q.Y * q.Y) + (q.Z * q.Z)));
        vector.Y = 2 * ((q.X * q.Y) + (q.W * q.Z));
        vector.Z = 2 * ((q.X * q.Z) - (q.W * q.Y));
    }
}
