// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

public abstract class RenderFeature : FrameworkObject
{
    public abstract void Render(RenderContext context, string stage, RenderObject renderable);
}

public abstract class RenderFeature<T> : RenderFeature
    where T : RenderObject
{
    private readonly ICommandQueue commands;
    protected readonly IGraphicsDevice Device = Game.Current.Services.Resolve<IGraphicsDevice>();

    public RenderFeature()
    {
        commands = Device.Factory.CreateCommandQueue();
    }

    public sealed override void Render(RenderContext context, string stage, RenderObject renderable)
    {
        try
        {
            commands.Begin();
            commands.SetFramebuffer(Device.SwapChain.Framebuffer);
            Render(context, stage, commands, (T)renderable);
        }
        finally
        {
            commands.End();
            Device.Submit(commands);
        }
    }

    public abstract void Render(RenderContext context, string stage, ICommandQueue commands, T renderable);
}
