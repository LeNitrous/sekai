// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Utils;

public static class MathUtils
{
    /// <summary>
    /// Convert a quaternion to euler angles (roll, pitch, yaw).
    /// </summary>
    public static Vector3 CreateEulerAnglesFromQuaternion(Quaternion q)
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
    /// Gets a quaternion's forward vector.
    /// </summary>
    public static Vector3 GetForward(Quaternion q)
    {
        float x = 2 * ((q.X * q.Z) + (q.W * q.Y));
        float y = 2 * ((q.Y * q.Z) - (q.W * q.X));
        float z = 1 - (2 * ((q.X * q.X) + (q.Y * q.Y)));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Gets a quaternion's right vector.
    /// </summary>
    public static Vector3 GetRight(Quaternion q)
    {
        float x = 1 - (2 * ((q.Y * q.Y) + (q.Z * q.Z)));
        float y = 2 * ((q.X * q.Y) + (q.W * q.Z));
        float z = 2 * ((q.X * q.Z) - (q.W * q.Y));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Gets a quaternion's up vector.
    /// </summary>
    public static Vector3 GetUp(Quaternion q)
    {
        float x = 2 * ((q.X * q.Y) - (q.W * q.Z));
        float y = 1 - (2 * ((q.X * q.X) + (q.Z * q.Z)));
        float z = 2 * ((q.Y * q.Z) + (q.W * q.X));

        return new Vector3(x, y, z);
    }

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
