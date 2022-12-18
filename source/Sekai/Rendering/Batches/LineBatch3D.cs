// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public sealed class LineBatch3D<T> : LineBatch<T>
    where T : unmanaged, IVertex3D, IColoredVertex
{
    public LineBatch3D(int maxLineCount)
        : base(maxLineCount)
    {
    }

    protected override Shader CreateShader() => new(shader);

    private static readonly string shader = @"
attrib vec3 a_Position;
attrib vec4 a_Color;
extern mat4 g_ProjMatrix;

void vert()
{
    SK_POSITION = g_ProjMatrix * vec4(a_Position.x, a_Position.y, a_Position.y, 1.0);
}

void frag()
{
    SK_COLOR0 = a_Color;
}";
}
