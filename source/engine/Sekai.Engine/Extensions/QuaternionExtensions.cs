// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Engine.Extensions;

public static class QuaternionExtensions
{
    /// <summary>
    /// Convert this quaternion to euler angles (roll, pitch, yaw).
    /// </summary>
    public static Vector3 ToEulerAngles(this Quaternion q)
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

        return new Vector3(roll, pitch, yaw);
    }

    /// <summary>
    /// Gets this quaternion's forward vector.
    /// </summary>
    public static Vector3 GetForward(this Quaternion q)
    {
        float x = 2 * ((q.X * q.Z) + (q.W * q.Y));
        float y = 2 * ((q.Y * q.Z) - (q.W * q.X));
        float z = 1 - (2 * ((q.X * q.X) + (q.Y * q.Y)));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Gets this quaternion's up vector.
    /// </summary>
    public static Vector3 GetUp(this Quaternion q)
    {
        float x = 2 * ((q.X * q.Y) - (q.W * q.Z));
        float y = 1 - (2 * ((q.X * q.X) + (q.Z * q.Z)));
        float z = 2 * ((q.Y * q.Z) + (q.W * q.X));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Gets this quaternion's right vector.
    /// </summary>
    public static Vector3 GetRight(this Quaternion q)
    {
        float x = 1 - (2 * ((q.Y * q.Y) + (q.Z * q.Z)));
        float y = 2 * ((q.X * q.Y) + (q.W * q.Z));
        float z = 2 * ((q.X * q.Z) - (q.W * q.Y));

        return new Vector3(x, y, z);
    }
}
