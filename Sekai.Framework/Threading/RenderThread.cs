// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;
using Veldrid;

namespace Sekai.Framework.Threading;

public sealed class RenderThread : GameThread
{
    private readonly Action<CommandList>? onRenderFrame;
    private readonly IGraphicsContext graphics;
    private readonly CommandList commands;
    private readonly bool isMainRenderThread;

    public RenderThread(IGraphicsContext graphics, string name = "unknown", Action<CommandList>? onRenderFrame = null)
        : this(graphics, name, onRenderFrame, false)
    {
    }

    internal RenderThread(IGraphicsContext graphics, string name = "unknown", Action<CommandList>? onRenderFrame = null, bool isMainRenderThread = false)
        : base($"Render ({name})")
    {
        commands = graphics.Device.ResourceFactory.CreateCommandList();
        OnExit += onExit;
        OnNewFrame += onNewFrame;
        this.graphics = graphics;
        this.onRenderFrame = onRenderFrame;
        this.isMainRenderThread = isMainRenderThread;
    }

    private void onNewFrame()
    {
        commands.Begin();
        onRenderFrame?.Invoke(commands);
        commands.End();
        graphics.Device.SubmitCommands(commands);

        if (isMainRenderThread)
        {
            graphics.Device.WaitForIdle();
            graphics.Device.SwapBuffers();
        }
    }

    private void onExit()
    {
        commands.Dispose();
    }
}
