// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Engine.Annotations;
using Sekai.Engine.Graphics;

#pragma warning disable CS0649

internal struct ColoredVertex : IVertex
{
    [VertexMember]
    public Vector2 Position;

    [VertexMember]
    public Vector2 TexCoord;

    [VertexMember]
    public uint Column;
}

#pragma warning restore CS0649
