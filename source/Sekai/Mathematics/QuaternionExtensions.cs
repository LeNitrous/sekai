// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Mathematics;
public static class QuaternionExtensions
{
    public static void RotationYawPitchRoll(this Quaternion quaternion, out double x, out double y, out double z)
    {
        float x2 = quaternion.X * quaternion.X;
        float y2 = quaternion.Y * quaternion.Y;
        float z2 = quaternion.Z * quaternion.Z;
        float w2 = quaternion.W * quaternion.W;

        x = (float)Math.Atan2(2 * (quaternion.X * quaternion.W + quaternion.Y * quaternion.Z), w2 + x2 - y2 - z2);
        y = (float)Math.Asin(2 * (quaternion.X * quaternion.Z - quaternion.W * quaternion.Y));
        z = (float)Math.Atan2(2 * (quaternion.Z * quaternion.W + quaternion.X * quaternion.Y), w2 - x2 - y2 + z2);
    }
}
