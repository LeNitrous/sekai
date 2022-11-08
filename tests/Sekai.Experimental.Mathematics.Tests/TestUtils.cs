// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Experimental.Mathematics.Tests;
internal static class TestUtils
{
    public static System.Numerics.Quaternion ConvertToSystemQuaternion(Quaternion val)
    {
        return new System.Numerics.Quaternion(val.X, val.Y, val.Z, val.W);
    }

    public static Quaternion ConvertFromSystemQuaternion(System.Numerics.Quaternion val)
    {
        return new Quaternion(val.X, val.Y, val.Z, val.W);
    }

    public static System.Numerics.Vector4 ConvertToSystemVec4(Vector4 val)
    {
        return new System.Numerics.Vector4(val.X, val.Y, val.Z, val.W);
    }

    public static Vector4 ConvertFromSystemVec4(System.Numerics.Vector4 val)
    {
        return new Vector4(val.X, val.Y, val.Z, val.W);
    }

    public static System.Numerics.Vector3 ConvertToSystemVec3(Vector3 val)
    {
        return new System.Numerics.Vector3(val.X, val.Y, val.Z);
    }

    public static Vector3 ConvertFromSystemVec3(System.Numerics.Vector3 val)
    {
        return new Vector3(val.X, val.Y, val.Z);
    }

    public static System.Numerics.Vector2 ConvertToSystemVec2(Vector2 val)
    {
        return new System.Numerics.Vector2(val.X, val.Y);
    }

    public static Vector2 ConvertFromSystemVec2(System.Numerics.Vector2 val)
    {
        return new Vector2(val.X, val.Y);
    }
}
