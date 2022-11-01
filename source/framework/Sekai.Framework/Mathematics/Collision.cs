// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sekai.Framework.Mathematics;

public static unsafe class Collision
{
    public static ContainmentType BoxContainsBox(ref BoundingBox box1, ref BoundingBox box2)
    {
        if (box1.Maximum.X < box2.Minimum.X || box1.Minimum.X > box2.Maximum.X ||
            box1.Maximum.Y < box2.Minimum.Y || box1.Minimum.Y > box2.Maximum.Y ||
            box1.Maximum.Z < box2.Minimum.Z || box1.Minimum.Z > box2.Maximum.Z)
        {
            return ContainmentType.Disjoint;
        }

        if (box1.Minimum.X <= box2.Minimum.X && box1.Maximum.X >= box2.Maximum.X &&
            box1.Minimum.Y <= box2.Minimum.Y && box1.Maximum.Y >= box2.Maximum.Y &&
            box1.Minimum.Z <= box2.Maximum.Z && box1.Maximum.Z >= box2.Maximum.Z)
        {
            return ContainmentType.Contains;
        }

        return ContainmentType.Intersects;
    }

    public static bool SphereContainsPoint(ref BoundingSphere sphere, ref Vector3 point)
    {
        return (sphere.Center - point).LengthSquared() <= sphere.Radius * sphere.Radius;
    }

    public static bool FrustumContainsPoint(ref BoundingFrustum frustum, ref Vector3 point)
    {
        var planes = (Plane*)Unsafe.AsPointer(ref frustum);

        for (int i = 0; i < 6; i++)
        {
            if (Plane.DotCoordinate(planes[i], point) < 0)
            {
                return false;
            }
        }

        return true;
    }

    public static ContainmentType FrustumContainsBox(ref BoundingFrustum frustum, ref BoundingBox box)
    {
        var planes = (Plane*)Unsafe.AsPointer(ref frustum);
        var result = ContainmentType.Contains;

        for (int i = 0; i < 6; i++)
        {
            var plane = planes[i];
            var positive = new Vector3(box.Minimum.X, box.Minimum.Y, box.Minimum.Z);
            var negative = new Vector3(box.Maximum.X, box.Maximum.Y, box.Maximum.Z);

            if (plane.Normal.X >= 0)
            {
                positive.X = box.Maximum.X;
                negative.X = box.Minimum.X;
            }

            if (plane.Normal.Y >= 0)
            {
                positive.Y = box.Maximum.Y;
                negative.Y = box.Minimum.Y;
            }

            if (plane.Normal.Z >= 0)
            {
                positive.Z = box.Maximum.Z;
                negative.Z = box.Minimum.Z;
            }

            if (Plane.DotCoordinate(plane, positive) < 0)
            {
                return ContainmentType.Disjoint;
            }

            if (Plane.DotCoordinate(plane, negative) < 0)
            {
                result = ContainmentType.Intersects;
            }
        }

        return result;
    }

    public static ContainmentType FrustumContainsSphere(ref BoundingFrustum frustum, ref BoundingSphere sphere)
    {
        var planes = (Plane*)Unsafe.AsPointer(ref frustum);
        var result = ContainmentType.Contains;

        for (int i = 0; i < 6; i++)
        {
            float distance = Plane.DotCoordinate(planes[i], sphere.Center);

            if (distance < -sphere.Radius)
            {
                return ContainmentType.Disjoint;
            }

            if (distance < sphere.Radius)
            {
                result = ContainmentType.Intersects;
            }
        }

        return result;
    }
}
