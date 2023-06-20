// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// An enumeration of blending types.
/// </summary>
public enum BlendType
{
    /// <summary>
    /// Each component is multiplied by zero.
    /// </summary>
    Zero,

    /// <summary>
    /// Each component is multiplied by one.
    /// </summary>
    One,

    /// <summary>
    /// Each component is multiplied by the matching component of the source color.
    /// </summary>
    SourceColor,

    /// <summary>
    /// Each component is multiplied by (1 - the matching component of the source color).
    /// </summary>
    OneMinusSourceColor,

    /// <summary>
    /// Each component is multiplied by the matching component of the destination color.
    /// </summary>
    DestinationColor,

    /// <summary>
    /// Each component is multiplied by (1 - the matching component of the destination color).
    /// </summary>
    OneMinusDestinationColor,

    /// <summary>
    /// Each component is multiplied by the source alpha component.
    /// </summary>
    SourceAlpha,

    /// <summary>
    /// Each component is multiplied by (1 - the source alpha component).
    /// </summary>
    OneMinusSourceAlpha,

    /// <summary>
    /// Each component is multiplied by the destination alpha component.
    /// </summary>
    DestinationAlpha,

    /// <summary>
    /// Each component is multiplied by (1 - the destination alpha component).
    /// </summary>
    OneMinusDestinationAlpha,
}
