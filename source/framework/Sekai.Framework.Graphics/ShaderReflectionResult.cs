// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public readonly struct ShaderReflectionResult
{
    public readonly IReadOnlyList<LayoutDescription> Layouts;
    public readonly IReadOnlyList<VertexElementDescription> VertexElements;

    public ShaderReflectionResult(IReadOnlyList<VertexElementDescription>? vertexElements = null, IReadOnlyList<LayoutDescription>? layouts = null)
    {
        VertexElements = vertexElements ?? Array.Empty<VertexElementDescription>();
        Layouts = layouts ?? Array.Empty<LayoutDescription>();
    }
}
