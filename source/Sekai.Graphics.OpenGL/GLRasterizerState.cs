// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed class GLRasterizerState : RasterizerState
{
    public readonly bool Scissor;
    public readonly bool Culling;
    public readonly TriangleFace CullingMode;
    public readonly FrontFaceDirection FrontFace;
    public readonly PolygonMode PolygonMode;

    public GLRasterizerState(RasterizerStateDescription description)
    {
        Scissor = description.ScissorTest;
        Culling = description.Culling != FaceCulling.None;
        CullingMode = Culling ? description.Culling.AsTriangleFace() : TriangleFace.Front;
        FrontFace = description.Winding.AsDirection();
        PolygonMode = description.Mode.AsPolygonMode();
    }

    public override void Dispose()
    {
    }
}
