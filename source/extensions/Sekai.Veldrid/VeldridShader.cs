// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridShader : VeldridGraphicsResource<Vd.Shader>, IShader
{
    public ShaderStage Stage { get; }
    public string EntryPoint { get; }

    public VeldridShader(ShaderDescription desc, Vd.Shader resource)
        : base(resource)
    {
        Stage = desc.Stage;
        EntryPoint = desc.EntryPoint;
    }
}
