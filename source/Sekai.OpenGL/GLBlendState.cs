// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed class GLBlendState : BlendState
{
    public readonly bool Enabled;
    public readonly bool Red;
    public readonly bool Green;
    public readonly bool Blue;
    public readonly bool Alpha;
    public readonly BlendingFactor SourceColor;
    public readonly BlendingFactor DestinationColor;
    public readonly BlendingFactor SourceAlpha;
    public readonly BlendingFactor DestinationAlpha;
    public readonly BlendEquationModeEXT ColorEquation;
    public readonly BlendEquationModeEXT AlphaEquation;

    public GLBlendState(BlendStateDescription description)
    {
        Enabled = description.Enabled;
        Red = (description.WriteMask & ColorWriteMask.Red) == ColorWriteMask.Red;
        Green = (description.WriteMask & ColorWriteMask.Green) == ColorWriteMask.Green;
        Blue = (description.WriteMask & ColorWriteMask.Blue) == ColorWriteMask.Blue;
        Alpha = (description.WriteMask & ColorWriteMask.Alpha) == ColorWriteMask.Alpha;
        SourceColor = description.SourceColor.AsFactor();
        SourceAlpha = description.SourceAlpha.AsFactor();
        DestinationColor = description.DestinationColor.AsFactor();
        DestinationAlpha = description.DestinationAlpha.AsFactor();
        ColorEquation = description.ColorOperation.AsEquation();
        AlphaEquation = description.AlphaOperation.AsEquation();
    }

    public override void Dispose()
    {
    }
}
