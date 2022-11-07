// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Experimental.Mathematics.Tests;
internal static class TestUtils
{
    public static System.Numerics.Vector4 ConvertToSystemVec4(Vector4 val)
    {
        return new System.Numerics.Vector4(val.X, val.Y, val.Z, val.W);
    }

    public static Vector4 ConvertFromSystemVec4(System.Numerics.Vector4 val)
    {
        return new Vector4(val.X, val.Y, val.Z, val.W);
    }
}
