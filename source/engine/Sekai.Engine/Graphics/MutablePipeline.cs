// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Graphics;

public class MutablePipeline : FrameworkObject
{
    private readonly IGraphicsDevice device = Game.Current.Services.Resolve<IGraphicsDevice>();
    private Dictionary<ComputePipelineDescription, IPipeline> computePipelines = null!;
    private Dictionary<GraphicsPipelineDescription, IPipeline> graphicsPipelines = null!;

    public IPipeline GetPipeline(GraphicsPipelineDescription description)
    {
        graphicsPipelines ??= new();

        if (!graphicsPipelines.TryGetValue(description, out var pipeline))
        {
            pipeline = device.Factory.CreatePipeline(ref description);
            graphicsPipelines.Add(description, pipeline);
        }

        return pipeline;
    }

    public IPipeline GetPipeline(ComputePipelineDescription description)
    {
        computePipelines ??= new();

        if (!computePipelines.TryGetValue(description, out var pipeline))
        {
            pipeline = device.Factory.CreatePipeline(ref description);
            computePipelines.Add(description, pipeline);
        }

        return pipeline;
    }

    protected override void Destroy()
    {
        if (graphicsPipelines != null)
        {
            foreach (var pipeline in graphicsPipelines.Values)
                pipeline.Dispose();
        }

        if (computePipelines != null)
        {
            foreach (var pipeline in computePipelines.Values)
                pipeline.Dispose();
        }
    }
}
