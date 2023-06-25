// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Graphics;

namespace Sekai.OpenGL;

internal sealed class GLInputLayout : InputLayout
{
    public int[] Strides { get; }

    public IReadOnlyList<InputLayoutDescription> Descriptions;

    public GLInputLayout(InputLayoutDescription[] descriptions)
    {
        Strides = new int[descriptions.Length];

        for (int i = 0; i < descriptions.Length; i++)
        {
            int stride = 0;

            for (int j = 0; j < descriptions[i].Members.Count; j++)
            {
                stride += descriptions[i].Members[j].Count * descriptions[i].Members[j].Format.SizeOfFormat();
            }

            Strides[i] = stride;
        }

        Descriptions = descriptions;
    }

    public override void Dispose()
    {
    }
}
