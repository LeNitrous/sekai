// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Extensions;
using Sekai.Framework.Graphics;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;
using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;

namespace Sekai.Framework.Platform;

public abstract class ViewHost : Host
{
    public event Action<bool>? OnFocusChanged;
    protected IView View;
    private IInputContext? input;
    private IGraphicsContext? graphics;
    private readonly GraphicsBackend backend = GraphicsBackend.Vulkan;

    protected ViewHost()
    {
        var opts = ViewOptions.Default;
        opts.API = backend.ToGraphicsAPI();
        opts.VSync = false;
        opts.ShouldSwapAutomatically = false;

        View = CreateView(opts);
    }

    protected abstract IView CreateView(ViewOptions opts);

    protected override void Initialize(Game game)
    {
        View.Initialize();

        input = View.CreateInput();
        graphics = View.CreateGraphics(backend);

        var threads = game.Services.Resolve<GameThreadManager>(true);
        var systems = game.Services.Resolve<GameSystemRegistry>(true);
        threads.Add(new RenderThread(graphics, render));

        game.Services.Cache(input);
        game.Services.Cache(graphics);

        View.Resize += size => graphics.Device.ResizeMainWindow((uint)size.X, (uint)size.Y);
        View.Closing += Dispose;
        View.FocusChanged += e => OnFocusChanged?.Invoke(e);

        void render(CommandList commands)
        {
            if (systems != null)
            {
                foreach (var system in systems.OfType<IRenderable>())
                    system.Render(commands);
            }

            game.Render(commands);
        }
    }

    protected override GameThread CreateMainThread()
    {
        var thread = new GameThread("Window", View.DoEvents);
        thread.OnExit += () => View.Reset();
        return thread;
    }

    protected sealed override void Destroy()
    {
        base.Destroy();
        input?.Dispose();
        graphics?.Dispose();
    }
}
