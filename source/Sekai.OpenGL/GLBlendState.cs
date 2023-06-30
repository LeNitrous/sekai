// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed class GLBlendState : BlendState
{
    public override bool Enabled { get; }
    public override BlendType SourceColor { get; }
    public override BlendType DestinationColor { get; }
    public override BlendOperation ColorOperation { get; }
    public override BlendType SourceAlpha { get; }
    public override BlendType DestinationAlpha { get; }
    public override BlendOperation AlphaOperation { get; }
    public override ColorWriteMask WriteMask { get; }
    public readonly bool Red;
    public readonly bool Green;
    public readonly bool Blue;
    public readonly bool Alpha;
    public readonly BlendingFactor SrcColor;
    public readonly BlendingFactor DstColor;
    public readonly BlendingFactor SrcAlpha;
    public readonly BlendingFactor DstAlpha;
    public readonly BlendEquationModeEXT ColorEquation;
    public readonly BlendEquationModeEXT AlphaEquation;

    public GLBlendState(BlendStateDescription description)
    {
        Enabled = description.Enabled;
        SourceColor = description.SourceColor;
        SourceAlpha = description.SourceAlpha;
        DestinationColor = description.DestinationColor;
        DestinationAlpha = description.DestinationAlpha;
        ColorOperation = description.ColorOperation;
        AlphaOperation = description.AlphaOperation;
        WriteMask = description.WriteMask;
        Red = (description.WriteMask & ColorWriteMask.Red) == ColorWriteMask.Red;
        Green = (description.WriteMask & ColorWriteMask.Green) == ColorWriteMask.Green;
        Blue = (description.WriteMask & ColorWriteMask.Blue) == ColorWriteMask.Blue;
        Alpha = (description.WriteMask & ColorWriteMask.Alpha) == ColorWriteMask.Alpha;
        SrcColor = description.SourceColor.AsFactor();
        SrcAlpha = description.SourceAlpha.AsFactor();
        DstColor = description.DestinationColor.AsFactor();
        DstAlpha = description.DestinationAlpha.AsFactor();
        ColorEquation = description.ColorOperation.AsEquation();
        AlphaEquation = description.AlphaOperation.AsEquation();
    }

    public override void Dispose()
    {
    }
}
