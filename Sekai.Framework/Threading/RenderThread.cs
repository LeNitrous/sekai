// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.ExceptionServices;
using Sekai.Framework.Graphics;

namespace Sekai.Framework.Threading;

public abstract class RenderThread : FrameworkThread
{
    private readonly bool isPrimaryRenderThread;
    private readonly CommandList commands = new();
    private readonly GraphicsContext context = (GraphicsContext)Game.Current.Services.Resolve<IGraphicsContext>(true);

    protected RenderThread(string name = "unknown")
        : base($"Render ({name})")
    {
    }

    internal RenderThread()
        : this("Main")
    {
        isPrimaryRenderThread = true;
    }

    protected abstract void OnRenderFrame(CommandList commands);

    protected sealed override void OnNewFrame()
    {
        commands.Start();

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
        }

        if (isPrimaryRenderThread)
        {
            context.Device.WaitForIdle();
            context.Device.SwapBuffers();
        }

        exceptionInfo?.Throw();
    }

    protected sealed override void Perform() => base.Perform();
    protected override void Destroy() => commands.Dispose();
}
