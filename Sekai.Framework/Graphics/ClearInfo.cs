// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Information determining how the current frame buffer should be cleared.
/// </summary>
public struct ClearInfo
{
    public static ClearInfo Default => new(new Color(0, 0, 0, 1f));

    /// <summary>
    /// The color to write to the frame buffer.
    /// </summary>
    public readonly Color Color;

    /// <summary>
    /// The color target index of the frame buffer.
    /// </summary>
    /// <remarks>
    /// This field is ignored if the current frame buffer is the main swap chain.
    /// </remarks>
    public readonly int Index;

    /// <summary>
    /// The depth to write to the frame buffer.
    /// </summary>
    public readonly double Depth;

    /// <summary>
    /// The stencil to write to the frame buffer.
    /// </summary>
    public readonly byte Stencil;

    public ClearInfo(Color color, int index = 0, double depth = 1d, byte stencil = 0)
    {
        Color = color;
        Index = index;
        Depth = depth;
        Stencil = stencil;
    }
}
