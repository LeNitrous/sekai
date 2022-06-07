// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Sekai.Framework.IO.Storage;
using Sekai.Framework.Logging;
using Sekai.Framework.Threading;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;

namespace Sekai.Framework;

public abstract class Game : FrameworkObject
{
    protected IView? View { get; private set; }
    protected IInputContext? Input { get; private set; }
    protected VirtualStorage? Storage { get; private set; }
    protected IGraphicsContext? Graphics { get; private set; }
    protected GameThreadManager? Threads { get; private set; }
    protected GameSystemRegistry? Systems { get; private set; }

    public void Run(GameOptions? options = null)
    {
        Logger.OnMessageLogged += new LogListenerConsole();
        options ??= GameOptions.Default;

        var viewOptions = new ViewOptions
        {
            API = options.Backend.ToGraphicsAPI(),
            VSync = options.VSync,
            ShouldSwapAutomatically = false,
        };

        var windowOptions = new WindowOptions(viewOptions)
        {
            Size = new Vector2D<int>(options.Size.Width, options.Size.Height),
            Title = options.Title,
            WindowBorder = WindowBorder.Fixed,
        };

        View = Window.IsViewOnly
            ? Window.GetView(viewOptions)
            : Window.Create(windowOptions);

        View.Load += initialize;
        View.Resize += resize;
        View.Closing += Dispose;
        View.Initialize();

        Graphics = new GraphicsContext(View, options.Backend);

        Threads = new GameThreadManager
        {
            ExecutionMode = options.ExecutionMode,
            FramesPerSecond = options.FramesPerSecond,
            UpdatePerSecond = options.UpdatePerSecond,
        };

        Threads.Add(new WindowThread(View));
        Threads.Add(new RenderThread(Graphics, "Main", Render, true));
        Threads.Add(new GameThread("Update", Update));
        Threads.Run();
    }

    private void initialize()
    {
        Input = View?.CreateInput();
        Storage = new VirtualStorage();
        Systems = new GameSystemRegistry();
        Load();
    }

    private void resize(Vector2D<int> size)
    {
        Graphics?.Device.ResizeMainWindow((uint)size.X, (uint)size.Y);
    }

    protected virtual void Load()
    {
    }

    protected virtual void Render(CommandList commands)
    {
    }

    protected virtual void Update()
    {
    }

    protected override void Destroy()
    {
        Threads?.Dispose();
        Storage?.Dispose();
        Input?.Dispose();
        Graphics?.Dispose();
        Systems?.Dispose();
    }
}
