// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Extensions;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;
using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;

namespace Sekai.Framework.Platform;

/// <summary>
/// A host backed by a <see cref="IView"/>.
/// </summary>
public class ViewHost : Host
{
    public event Action<bool>? OnFocusChanged;
    protected IView View;
    private IInputContext? input;
    private IGraphicsContext? graphics;

    internal ViewHost(HostOptions? options = null)
        : base(options)
    {
        var opts = ViewOptions.Default;
        opts.API = Options.Renderer.ToGraphicsAPI();
        opts.VSync = false;
        opts.ShouldSwapAutomatically = false;
        View = CreateView(opts);
    }

    /// <summary>
    /// Creates the underlying view for this host.
    /// </summary>
    protected virtual IView CreateView(ViewOptions opts) => Window.GetView(opts);
    protected override FrameworkThreadManager CreateThreadManager() => new ViewThreadManager(View);

    protected override void Initialize(Game game)
    {
        Logger.OnMessageLogged += new LogListenerConsole();

        View.Initialize();

        game.Services.Cache(input = View.CreateInput());
        game.Services.Cache(graphics = View.CreateGraphics(Options.Renderer));

        var threads = game.Services.Resolve<FrameworkThreadManager>(true);
        threads.Add(new GameRenderThread(game, graphics));

        View.Resize += size => graphics.Device.ResizeMainWindow((uint)size.X, (uint)size.Y);
        View.Closing += Dispose;
        View.FocusChanged += e => OnFocusChanged?.Invoke(e);
    }

    protected sealed override void Destroy()
    {
        base.Destroy();
        input?.Dispose();
        graphics?.Dispose();
    }

    private class ViewThreadManager : FrameworkThreadManager
    {
        private readonly IView view;
        private ViewMainThread? mainThread;

        public ViewThreadManager(IView view)
        {
            this.view = view;
        }

        protected override MainThread CreateMainThread() => mainThread = new ViewMainThread();

        protected override void Initialize()
        {
            if (mainThread != null)
                mainThread.View = view;
        }
    }

    private class ViewMainThread : MainThread
    {
        public IView? View { get; set; }

        protected override void OnNewFrame() => View?.DoEvents();

        protected override void Destroy()
        {
            base.Destroy();
            View?.DoEvents();
            View?.Reset();
        }
    }

    private class GameRenderThread : RenderThread
    {
        private readonly Game game;
        private readonly GameSystemRegistry systems;

        public GameRenderThread(Game game, IGraphicsContext graphics)
            : base(graphics)
        {
            this.game = game;
            systems = game.Services.Resolve<GameSystemRegistry>(true);
        }

        protected override void OnRenderFrame(CommandList commands)
        {
            foreach (var system in systems.OfType<IRenderable>())
                system.Render(commands);

            game.Render(commands);
        }
    }
}
