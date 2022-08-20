// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessShader : FrameworkObject, IShader
{
    public ShaderStage Stage { get; }
    public string EntryPoint { get; }

    public HeadlessShader(ShaderStage stage, string entryPoint)
    {
        Stage = stage;
        EntryPoint = entryPoint;
    }
}
