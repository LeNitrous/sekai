// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Shaders;

[Flags]
public enum ShaderStage : byte
{
    None = 0x0,
    Vertex = 0x1,
    Geometry = 0x2,
    TessellationControl = 0x4,
    TessellationEvaluation = 0x8,
    Fragment = 0x10,
    Compute = 0x20,
}
