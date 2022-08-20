// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface IPipeline : IGraphicsResource
{
    /// <summary>
    /// Gets what kind of pipeline this resource is.
    /// </summary>
    PipelineKind Kind { get; }
}
