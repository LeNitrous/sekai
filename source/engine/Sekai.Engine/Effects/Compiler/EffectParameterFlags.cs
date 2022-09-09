// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Engine.Effects.Compiler;

[Flags]
public enum EffectParameterFlags
{
    None,
    Buffer = 1 << 0,
    Uniform = 1 << 1,
    Image = 1 << 2,
    Texture = 1 << 3,
    Sampler = 1 << 4,
    Cubemap = 1 << 5,
    Texture1D = 1 << 6,
    Texture2D = 1 << 7,
    Texture3D = 1 << 8,
}
