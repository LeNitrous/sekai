// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.ExceptionServices;
using Sekai.Framework.Graphics;
using Veldrid;

namespace Sekai.Framework.Threading;

public abstract class RenderThread : FrameworkThread
{
    private readonly bool isPrimaryRenderThread;
    private readonly CommandList commands;
    private readonly IGraphicsContext graphics;

    protected RenderThread(IGraphicsContext graphics, string name = "unknown")
        : base($"Render ({name})")
    {
        this.graphics = graphics;
        commands = graphics.Device.ResourceFactory.CreateCommandList();
    }

    internal RenderThread(IGraphicsContext graphics)
        : this(graphics, "Main")
    {
        isPrimaryRenderThread = true;
    }

    protected abstract void OnRenderFrame(CommandList commands);

    protected sealed override void OnNewFrame()
    {
        commands.Begin();

        ExceptionDispatchInfo? exceptionInfo = null;

        try
        {
            OnRenderFrame(commands);
        }
        catch (Exception e)
        {
            exceptionInfo = ExceptionDispatchInfo.Capture(e);
        }
        finally
        {
            commands.End();
            graphics.Device.SubmitCommands(commands);
        }

        if (isPrimaryRenderThread)
        {
            graphics.Device.WaitForIdle();
            graphics.Device.SwapBuffers();
        }

        exceptionInfo?.Throw();
    }

    protected sealed override void Perform() => base.Perform();
    protected override void Destroy() => commands.Dispose();
}
