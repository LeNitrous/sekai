// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Framework.Mathematics;

namespace Sekai.Framework.Extensions;

public static class MatrixExtensions
{
    /// <summary>
    /// Decomposes a matrix to its position component.
    /// </summary>
    public static bool Decompose(this Matrix4x4 m, out Vector3 position)
    {
        position.X = m.M41;
        position.Y = m.M42;
        position.Z = m.M43;
        return true;
    }

    /// <summary>
    /// Decomposes a matrix to its position, rotation, and scale components.
    /// </summary>
    public static bool Decompose(this Matrix4x4 m, out Vector3 position, out Matrix4x4 rotation, out Vector3 scale)
    {
        Decompose(m, out position);

        scale.X = MathF.Sqrt((m.M11 * m.M11) + (m.M12 * m.M12) + (m.M13 * m.M13));
        scale.Y = MathF.Sqrt((m.M21 * m.M21) + (m.M22 * m.M22) + (m.M23 * m.M23));
        scale.Z = MathF.Sqrt((m.M31 * m.M31) + (m.M32 * m.M32) + (m.M33 * m.M33));

        if (MathF.Abs(scale.X) < MathUtil.ZeroTolerance ||
            MathF.Abs(scale.Y) < MathUtil.ZeroTolerance ||
            MathF.Abs(scale.Z) < MathUtil.ZeroTolerance)
        {
            rotation = Matrix4x4.Identity;
            return false;
        }

        rotation = Matrix4x4.Identity;
        var at = new Vector3(m.M31 / scale.Z, m.M32 / scale.Z, m.M33 / scale.Z);
        var up = Vector3.Cross(at, new Vector3(m.M11 / scale.X, m.M12 / scale.X, m.M13 / scale.X));
        var right = Vector3.Cross(up, at);

        rotation.M11 = right.X;
        rotation.M12 = right.Y;
        rotation.M13 = right.Z;
        rotation.M21 = up.X;
        rotation.M22 = up.Y;
        rotation.M23 = up.Z;
        rotation.M31 = at.X;
        rotation.M32 = at.Y;
        rotation.M33 = at.Z;

        return true;
    }

    /// <summary>
    /// Decomposes a matrix to its position, rotation, and scale components.
    /// </summary>
    public static bool Decompose(this Matrix4x4 m, out Vector3 position, out Quaternion rotation, out Vector3 scale)
    {
        Decompose(m, out position, out Matrix4x4 rotationMatrix, out scale);
        ToQuaternion(rotationMatrix, out rotation);
        return true;
    }

    /// <summary>
    /// Converts a rotation matrix to a quaternion.
    /// </summary>
    public static void ToQuaternion(this Matrix4x4 m, out Quaternion quat)
    {
        float sqrt;
        float half;
        float scale = m.M11 + m.M22 + m.M33;

        if (scale > 0.0f)
        {
            sqrt = MathF.Sqrt(scale + 1.0f);
            quat.W = sqrt * 0.5f;

            sqrt = 0.5f / sqrt;
            quat.X = (m.M23 - m.M32) * sqrt;
            quat.Y = (m.M31 - m.M13) * sqrt;
            quat.Z = (m.M12 - m.M21) * sqrt;
        }
        else if ((m.M11 >= m.M22) && (m.M11 >= m.M33))
        {
            sqrt = MathF.Sqrt(1.0f + m.M11 - m.M22 - m.M33);
            half = 0.5f / sqrt;

            quat.X = 0.5f * sqrt;
            quat.Y = (m.M12 + m.M21) * half;
            quat.Z = (m.M13 + m.M31) * half;
            quat.W = (m.M23 - m.M32) * half;
        }
        else if (m.M22 > m.M33)
        {
            sqrt = MathF.Sqrt(1.0f + m.M22 - m.M11 - m.M33);
            half = 0.5f / sqrt;

            quat.X = (m.M21 + m.M12) * half;
            quat.Y = 0.5f * sqrt;
            quat.Z = (m.M32 + m.M23) * half;
            quat.W = (m.M31 - m.M13) * half;
        }
        else
        {
            sqrt = MathF.Sqrt(1.0f + m.M33 - m.M11 - m.M22);
            half = 0.5f / sqrt;

            quat.X = (m.M31 + m.M13) * half;
            quat.Y = (m.M32 + m.M23) * half;
            quat.Z = 0.5f * sqrt;
            quat.W = (m.M12 - m.M21) * half;
        }
    }
}
