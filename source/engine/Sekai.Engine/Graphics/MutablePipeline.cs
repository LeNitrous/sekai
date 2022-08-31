// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Graphics;

public class MutablePipeline<T> : FrameworkObject
    where T : struct
{
    private readonly IGraphicsDevice device;
    private readonly Dictionary<T, IPipeline> pipelines = new();
    private readonly Func<IGraphicsDevice, T, IPipeline> createPipelineFunc;

    public MutablePipeline(IGraphicsDevice device, Func<IGraphicsDevice, T, IPipeline> createPipelineFunc)
    {
        this.device = device;
        this.createPipelineFunc = createPipelineFunc;
    }

    public IPipeline GetPipeline(T description)
    {
        if (!pipelines.TryGetValue(description, out var pipeline))
        {
            pipeline = createPipelineFunc(device, description);
            pipelines.Add(description, pipeline);
        }

        return pipeline;
    }
}
