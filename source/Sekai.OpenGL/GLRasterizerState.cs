// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed class GLRasterizerState : RasterizerState
{
    public override FillMode Mode { get; }
    public override bool Scissor { get; }
    public override FaceWinding Winding { get; }
    public override FaceCulling Culling { get; }
    public TriangleFace CullingMode { get; }
    public FrontFaceDirection FrontFace { get; }
    public PolygonMode PolygonMode { get; }
    public bool CullingEnabled => Culling != FaceCulling.None;

    public GLRasterizerState(RasterizerStateDescription description)
    {
        Mode = description.Mode;
        Scissor = description.Scissor;
        Winding = description.Winding;
        Culling = description.Culling;
        CullingMode = CullingEnabled ? description.Culling.AsTriangleFace() : TriangleFace.Front;
        FrontFace = description.Winding.AsDirection();
        PolygonMode = description.Mode.AsPolygonMode();
    }

    public override void Dispose()
    {
    }
}
