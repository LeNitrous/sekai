// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Extensions;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Threading;
using Silk.NET.Windowing;

namespace Sekai.Framework.Platform;

/// <summary>
/// A host backed by a <see cref="IView"/>.
/// </summary>
public class ViewHost : Host
{
    public event Action<bool>? OnFocusChanged;
    protected IView View;

    internal ViewHost(HostOptions? options = null)
        : base(options)
    {
        var opts = ViewOptions.Default;
        opts.API = Options.Renderer.ToSilk();
        opts.VSync = false;
        opts.ShouldSwapAutomatically = false;

        View = CreateView(opts);
        View.Closing += Exit;
        View.FocusChanged += e => OnFocusChanged?.Invoke(e);
    }

    /// <summary>
    /// Creates the underlying view for this host.
    /// </summary>
    protected virtual IView CreateView(ViewOptions opts) => Window.GetView(opts);

    protected override FrameworkThreadManager CreateThreadManager() => new ViewThreadManager(View);

    protected override void Initialize(Game game)
    {
        var graphics = (GraphicsContext)game.Services.Resolve<IGraphicsContext>(true);
        View.Resize += size => RenderThread?.Post(() => graphics.Device.ResizeMainWindow((uint)size.X, (uint)size.Y));

        Logger.OnMessageLogged += new LogListenerConsole();

        View.Initialize();

        View.Resize += size => graphics.Device.ResizeMainWindow((uint)size.X, (uint)size.Y);
        View.Closing += Dispose;
        View.FocusChanged += e => OnFocusChanged?.Invoke(e);
    }

    protected sealed override IGraphicsContext CreateGraphicsContext(Graphics.GraphicsAPI api)
    {
        View.Initialize();
        return new GraphicsContext(View, api);
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

        protected override void OnNewFrame()
        {
            View?.DoEvents();
        }

        protected override void Destroy()
        {
            base.Destroy();
            View?.Reset();
        }
    }
}
