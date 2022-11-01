// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines the stencil information used during stencil tests.
/// </summary>
public readonly struct StencilInfo : IEquatable<StencilInfo>
{
    /// <summary>
    /// Whether stencil testing should occur.
    /// </summary>
    public readonly bool StencilTest;

    /// <summary>
    /// The stencil test function.
    /// </summary>
    public readonly ComparisonKind Function;

    /// <summary>
    /// The stencil test value to compare against.
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// The stencil mask.
    /// </summary>
    public readonly int Mask;

    /// <summary>
    /// The operation to perform on the stencil buffer when the stencil test fails.
    /// </summary>
    public readonly StencilOperation StencilFailOperation;

    /// <summary>
    /// The operation to perform on the stencil buffer when the depth test fails.
    /// </summary>
    public readonly StencilOperation DepthTestFailOperation;

    /// <summary>
    /// The operation to perform on the stencil buffer when both the stencil and depth tests pass.
    /// </summary>
    public readonly StencilOperation PassOperation;

    public StencilInfo(bool stencilTest = true, ComparisonKind function = ComparisonKind.Always, int value = 1, int mask = 0xff,
                       StencilOperation stencilFail = StencilOperation.Keep, StencilOperation depthFail = StencilOperation.Keep, StencilOperation pass = StencilOperation.Replace)
    {
        Mask = mask;
        Value = value;
        Function = function;
        StencilTest = stencilTest;
        PassOperation = pass;
        DepthTestFailOperation = depthFail;
        StencilFailOperation = stencilFail;
    }

    public override bool Equals(object? obj)
    {
        return obj is StencilInfo other && Equals(other);

    }

    public bool Equals(StencilInfo other)
    {
        return StencilTest == other.StencilTest &&
               Function == other.Function &&
               Value == other.Value &&
               Mask == other.Mask &&
               StencilFailOperation == other.StencilFailOperation &&
               DepthTestFailOperation == other.DepthTestFailOperation &&
               PassOperation == other.PassOperation;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StencilTest, Function, Value, Mask, StencilFailOperation, DepthTestFailOperation, PassOperation);
    }

    public static bool operator ==(StencilInfo left, StencilInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(StencilInfo left, StencilInfo right)
    {
        return !(left == right);
    }
}
