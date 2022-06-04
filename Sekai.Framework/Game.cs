// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using Sekai.Framework.Graphics;
using Sekai.Framework.IO.Storage;
using Sekai.Framework.Logging;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Sekai.Framework;

public abstract class Game : FrameworkComponent
{
    protected IInputContext? Input { get; private set; }
    protected VirtualStorage? Storage { get; private set; }
    protected IGraphicsContext? Graphics { get; private set; }
    private readonly Stopwatch clock = new();

    public void Run()
    {
        Storage = new VirtualStorage();
        Graphics = new GraphicsContext();
        Graphics.View.Load += initialize;
        Graphics.View.Render += _ => render();
        Graphics.View.Update += _ => update();
        Graphics.View.Closing += Dispose;
        Graphics.View.Run();
    }

    protected virtual void Load()
    {
    }

    protected virtual void Draw()
    {
    }

    protected virtual void Update(TimeSpan elapsed)
    {
    }

    private void initialize()
    {
        Logger.OnMessageLogged += new LogListenerConsole();
        Input = Graphics?.View.CreateInput();
        Load();
    }

    private void render()
    {
        if (Graphics is GraphicsContext context && context.IsLoaded)
        {
            context.Commands.Begin();

            try
            {
                Draw();
            }
            catch (Exception e)
            {
                Logger.Error(@"An exception has occured while rendering.", e);
            }

            context.Commands.End();
            context.Device.SubmitCommands(context.Commands);
            context.Device.WaitForIdle();
            context.Device.SwapBuffers();
        }
    }

    private void update()
    {
        clock.Restart();

        try
        {
            Update(clock.Elapsed);
        }
        catch (Exception e)
        {
            Logger.Error(@"An exception has occured while updating.", e);
        }
    }

    protected override void Destroy()
    {
        Input?.Dispose();
        Storage?.Dispose();
        Graphics?.Dispose();
    }
}
