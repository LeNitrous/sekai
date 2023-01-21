// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Graphics.Shaders;

namespace Sekai.Null.Graphics;

internal class NullShader : NativeShader
{
    public override ShaderType Type { get; } = ShaderType.Graphic;
    public override IReadOnlyList<IUniform> Uniforms { get; } = Array.Empty<IUniform>();
}
