// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Dummy;

internal class DummyPipeline : FrameworkObject, IPipeline
{
    public PipelineKind Kind { get; }

    public DummyPipeline(PipelineKind kind)
    {
        Kind = kind;
    }
}
