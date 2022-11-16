// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Mathematics;
public static class Vector3Extensions
{
    public static void Normalize(this Vector3 vector)
    {
        float length = vector.Length();
        vector.X /= length;
        vector.Y /= length;
        vector.Z /= length;
    }
}
