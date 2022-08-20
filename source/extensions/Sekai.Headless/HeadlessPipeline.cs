// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessPipeline : FrameworkObject, IPipeline
{
    public PipelineKind Kind { get; }

    public HeadlessPipeline(PipelineKind kind)
    {
        Kind = kind;
    }
}
