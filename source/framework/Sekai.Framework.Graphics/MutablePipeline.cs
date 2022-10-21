// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public class MutablePipeline<T> : FrameworkObject
    where T : struct, IEquatable<T>
{
    private readonly IGraphicsDevice device;
    private Dictionary<T, IPipeline>? pipelines;

    public MutablePipeline(IGraphicsDevice device)
    {
        this.device = device;
    }

    public IPipeline GetPipeline(T description)
    {
        pipelines ??= new();

        if (!pipelines.TryGetValue(description, out var pipeline))
        {
            if (description is GraphicsPipelineDescription graphicsPipelineDescription)
            {
                pipeline = device.Factory.CreatePipeline(ref graphicsPipelineDescription);
            }
            else if (description is ComputePipelineDescription computePipelineDescription)
            {
                pipeline = device.Factory.CreatePipeline(ref computePipelineDescription);
            }
            else
            {
                throw new InvalidOperationException();
            }

            pipelines.Add(description, pipeline);
        }

        return pipeline;
    }

    protected override void Destroy()
    {
        if (pipelines is null)
            return;

        foreach ((_, var pipeline) in pipelines)
            pipeline.Dispose();
    }
}
