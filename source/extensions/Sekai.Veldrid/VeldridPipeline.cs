// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridPipeline : VeldridGraphicsResource<Vd.Pipeline>, IPipeline
{
    public PipelineKind Kind { get; }

    public VeldridPipeline(PipelineKind kind, Vd.Pipeline resource)
        : base(resource)
    {
        Kind = kind;
    }
}
