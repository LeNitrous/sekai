// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Framework.Graphics.Vertices;

public struct TexturedVertex2D : IVertex
{
    [VertexMember(VertexMemberFormat.Float2)]
    public Vector2 Position;

    [VertexMember(VertexMemberFormat.Float2)]
    public Vector2 TextureCoordinates;

    [VertexMember(VertexMemberFormat.Float4)]
    public Color Color;
}
