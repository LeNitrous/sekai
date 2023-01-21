// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Shaders;

namespace Sekai.Null.Graphics;

internal class NullShaderTranspiler : ShaderTranspiler
{
    protected override string Header { get; } = string.Empty;
}
