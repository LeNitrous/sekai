// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Influences components in a blend operation.
/// </summary>
public enum BlendFactor
{
    /// <summary>
    /// Each component is multiplied by 0.
    /// </summary>
    Zero,

    /// <summary>
    /// Each component is multiplied by 1.
    /// </summary>
    One,

    /// <summary>
    /// Each component is multiplied by the source alpha component.
    /// </summary>
    SourceAlpha,

    /// <summary>
    /// Each component is multiplied by the inverse of the source alpha component (1 - source alpha).
    /// </summary>
    InverseSourceAlpha,

    /// <summary>
    /// Each component is multiplied by the destination alpha componenet.
    /// </summary>
    DestinationAlpha,

    /// <summary>
    /// Each component is multiplied by the inverse of the destination alpha component (1 - destination alpha).
    /// </summary>
    InverseDestinationAlpha,

    /// <summary>
    /// Each component is multiplied by the matching component of the source color.
    /// </summary>
    SourceColor,

    /// <summary>
    /// Each component is multiplied by the inverse of the matching component of the source color (1 - matching component of the source color).
    /// </summary>
    InverseSourceColor,

    /// <summary>
    /// Each component is multiplied by the matching component of the destination color.
    /// </summary>
    DestinationColor,

    /// <summary>
    /// Each component is multplied by the inverse of the matching component of the destination color (1 - matching component of the destination color).
    /// </summary>
    InverseDestinationColor,

    /// <summary>
    /// Each component is multiplied by the matching component in constant factor specified in <see cref="BlendInfo.Factor"/>.
    /// </summary>
    BlendFactor,

    /// <summary>
    /// Each component is multiplied by the inverse of the matching component in constant factor specified in <see cref="BlendInfo.Factor"/>. (1 - matching component for <see cref="BlendInfo.Factor"/>).
    /// </summary>
    InverseBlendFactor,
}
