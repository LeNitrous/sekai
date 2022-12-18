// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal static class GLUtils
{
    public static PrimitiveType ToPrimitiveType(PrimitiveTopology topology) => topology switch
    {
        PrimitiveTopology.Triangles => PrimitiveType.Triangles,
        PrimitiveTopology.TriangleStrip => PrimitiveType.TriangleStrip,
        PrimitiveTopology.Lines => PrimitiveType.Lines,
        PrimitiveTopology.LineStrip => PrimitiveType.LineStrip,
        PrimitiveTopology.Points => PrimitiveType.Points,
        _ => throw new ArgumentException($"Unsupported primitive topology: {topology}", nameof(topology)),
    };

    public static DepthFunction ToDepthFunction(ComparisonKind kind) => kind switch
    {
        ComparisonKind.Never => DepthFunction.Never,
        ComparisonKind.LessThan => DepthFunction.Less,
        ComparisonKind.LessThanOrEqual => DepthFunction.Lequal,
        ComparisonKind.Equal => DepthFunction.Equal,
        ComparisonKind.GreaterThanOrEqual => DepthFunction.Gequal,
        ComparisonKind.GreaterThan => DepthFunction.Greater,
        ComparisonKind.NotEqual => DepthFunction.Notequal,
        ComparisonKind.Always => DepthFunction.Always,
        _ => throw new ArgumentException($"Unsupported depth test function: {kind}", nameof(kind)),
    };

    public static StencilFunction ToStencilFunction(ComparisonKind kind) => kind switch
    {
        ComparisonKind.Never => StencilFunction.Never,
        ComparisonKind.LessThan => StencilFunction.Less,
        ComparisonKind.LessThanOrEqual => StencilFunction.Lequal,
        ComparisonKind.Equal => StencilFunction.Equal,
        ComparisonKind.GreaterThanOrEqual => StencilFunction.Gequal,
        ComparisonKind.GreaterThan => StencilFunction.Greater,
        ComparisonKind.NotEqual => StencilFunction.Notequal,
        ComparisonKind.Always => StencilFunction.Always,
        _ => throw new ArgumentException($"Unsupported stencil function: {kind}", nameof(kind)),
    };

    public static StencilOp ToStencilOperation(StencilOperation operation) => operation switch
    {
        StencilOperation.Zero => StencilOp.Zero,
        StencilOperation.Invert => StencilOp.Invert,
        StencilOperation.Keep => StencilOp.Keep,
        StencilOperation.Replace => StencilOp.Replace,
        StencilOperation.Increment => StencilOp.Incr,
        StencilOperation.Decrement => StencilOp.Decr,
        StencilOperation.IncrementWrapped => StencilOp.IncrWrap,
        StencilOperation.DecrementWrapped => StencilOp.DecrWrap,
        _ => throw new ArgumentException($"Unsupported stencil operation: {operation}", nameof(operation)),
    };

    public static BlendEquationModeEXT ToBlendEquationModeEXT(BlendingEquation equation) => equation switch
    {
        BlendingEquation.Add => BlendEquationModeEXT.FuncAdd,
        BlendingEquation.Minimum => BlendEquationModeEXT.Min,
        BlendingEquation.Maximum => BlendEquationModeEXT.Max,
        BlendingEquation.Subtract => BlendEquationModeEXT.FuncSubtract,
        BlendingEquation.SubtractReverse => BlendEquationModeEXT.FuncReverseSubtract,
        _ => throw new ArgumentException($"Unsupported blending equation: {equation}", nameof(equation)),
    };

    public static BlendingFactor ToBlendingFactor(BlendingType type) => type switch
    {
        BlendingType.Zero => BlendingFactor.Zero,
        BlendingType.One => BlendingFactor.One,
        BlendingType.ConstantAlpha => BlendingFactor.ConstantAlpha,
        BlendingType.ConstantColor => BlendingFactor.ConstantColor,
        BlendingType.DestinationAlpha => BlendingFactor.DstAlpha,
        BlendingType.DestinationColor => BlendingFactor.DstColor,
        BlendingType.OneMinusConstantAlpha => BlendingFactor.OneMinusConstantAlpha,
        BlendingType.OneMinusConstantColor => BlendingFactor.OneMinusConstantColor,
        BlendingType.OneMinusDestinationAlpha => BlendingFactor.OneMinusDstAlpha,
        BlendingType.OneMinusDestinationColor => BlendingFactor.OneMinusDstColor,
        BlendingType.OneMinusSourceAlpha => BlendingFactor.OneMinusSrcAlpha,
        BlendingType.OneMinusSourceColor => BlendingFactor.OneMinusSrcColor,
        BlendingType.SourceAlpha => BlendingFactor.SrcAlpha,
        BlendingType.SourceColor => BlendingFactor.SrcColor,
        _ => throw new ArgumentException($"Unsupported blending factor: {type}", nameof(type)),
    };
}
