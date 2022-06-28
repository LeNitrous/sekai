// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct StencilInfo : IEquatable<StencilInfo>
{
    public bool StencilTest { get; set; }
    public StencilBehavior Front { get; set; }
    public StencilBehavior Back { get; set; }
    public byte ReadMask { get; set; }
    public byte WriteMask { get; set; }
    public int Reference { get; set; }

    public bool Equals(StencilInfo other)
    {
        return StencilTest == other.StencilTest
            && Front == other.Front
            && Back == other.Back
            && ReadMask == other.ReadMask
            && WriteMask == other.WriteMask
            && Reference == other.Reference;
    }

    public override bool Equals(object? obj) => obj is StencilInfo info && Equals(info);

    public override int GetHashCode() => HashCode.Combine(StencilTest, Front, Back, ReadMask, WriteMask, Reference);

    public static bool operator ==(StencilInfo left, StencilInfo right) => left.Equals(right);

    public static bool operator !=(StencilInfo left, StencilInfo right) => !(left == right);
}
