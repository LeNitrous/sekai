// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed class GLDepthStencilState : DepthStencilState
{
    public bool DepthMask;
    public bool DepthEnabled;
    public bool StencilEnabled;
    public byte StencilReadMask;
    public byte StencilWriteMask;
    public readonly DepthFunction DepthFunction;
    public readonly StencilOp FrontDepthFail;
    public readonly StencilOp FrontStencilPass;
    public readonly StencilOp FrontStencilFail;
    public readonly StencilFunction FrontFunction;
    public readonly StencilOp BackDepthFail;
    public readonly StencilOp BackStencilPass;
    public readonly StencilOp BackStencilFail;
    public readonly StencilFunction BackFunction;

    public GLDepthStencilState(DepthStencilStateDescription description)
    {
        DepthMask = description.DepthMask;
        DepthEnabled = description.DepthTest;
        StencilEnabled = description.StencilTest;
        StencilReadMask = description.StencilReadMask;
        StencilWriteMask = description.StencilWriteMask;
        DepthFunction = (DepthFunction)description.DepthComparison.AsEnum();
        FrontFunction = (StencilFunction)description.Front.Comparison.AsEnum();
        FrontDepthFail = description.Front.DepthFail.AsOperation();
        FrontStencilPass = description.Front.StencilPass.AsOperation();
        FrontStencilFail = description.Front.StencilFail.AsOperation();
        BackFunction = (StencilFunction)description.Back.Comparison.AsEnum();
        BackDepthFail = description.Back.DepthFail.AsOperation();
        BackStencilPass = description.Back.StencilPass.AsOperation();
        BackStencilFail = description.Back.StencilFail.AsOperation();
    }

    public override void Dispose()
    {
    }
}
