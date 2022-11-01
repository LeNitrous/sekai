// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public enum ComparisonKind
{
    /// <summary>
    /// The test always fails.
    /// </summary>
    Never,

    /// <summary>
    /// The test passes when the incoming value is less than the value in the buffer.
    /// </summary>
    LessThan,

    /// <summary>
    /// The test passes when the incoming value is less than or equal the value in the buffer.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// The test passes when the incoming value is equal the value in the buffer.
    /// </summary>
    Equal,

    /// <summary>
    /// The test passes when the incoming value is greater than or equal the value in the buffer.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// The test passes when the incoming value is greater than the value in the buffer.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The test passes when the incoming value is not equal the value in the buffer.
    /// </summary>
    NotEqual,

    /// <summary>
    /// The test always passes.
    /// </summary>
    Always,
}
