// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;

namespace Sekai.Headless.Graphics;

internal sealed class DummyRasterizerState : RasterizerState
{
    public override FaceCulling Culling { get; }
    public override FaceWinding Winding { get; }
    public override FillMode Mode { get; }
    public override bool Scissor { get; }

    public override void Dispose()
    {
    }
}
