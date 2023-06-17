// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Flags that determine which color channels to draw to.
/// </summary>
[Flags]
public enum ColorWriteMask
{
    /// <summary>
    /// Draw to no channels.
    /// </summary>
    None = 0,

    /// <summary>
    /// Draw to the red channel.
    /// </summary>
    Red = 1 << 0,

    /// <summary>
    /// Draw to the green channel.
    /// </summary>
    Green = 1 << 1,

    /// <summary>
    /// Draw to the blue channel.
    /// </summary>
    Blue = 1 << 2,

    /// <summary>
    /// Draw to the alpha channel.
    /// </summary>
    Alpha = 1 << 3,

    /// <summary>
    /// Draw to all channels.
    /// </summary>
    All = Red | Green | Blue | Alpha,
}
