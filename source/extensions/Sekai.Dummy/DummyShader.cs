// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Dummy;

internal class DummyShader : FrameworkObject, IShader
{
    public ShaderStage Stage { get; }
    public string EntryPoint { get; }

    public DummyShader(ShaderStage stage, string entryPoint)
    {
        Stage = stage;
        EntryPoint = entryPoint;
    }
}
