// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework;
using Sekai.Framework.Graphics;
using Sekai.Framework.Logging;
using Sekai.Framework.Threading;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Threading;

public sealed class ThreadController : FrameworkObject
{
    /// <summary>
    /// Whether this thread controller is currently running.
    /// </summary>
    public bool IsRunning => Window.IsRunning;

    /// <summary>
    /// Whether this thread controller should stop when an unhandled exception occurs.
    /// </summary>
    public bool AbortOnUnhandledException = true;

    /// <summary>
    /// Whether this thread controller should stop when an unobserved exception occurs.
    /// </summary>
    public bool AbortOnUnobservedException = false;

    /// <summary>
    /// Called when a thread has been added.
    /// </summary>
    public event Action<FrameworkThread> OnThreadAdded = null!;

    /// <summary>
    /// Called when a thread has been removed.
    /// </summary>
    public event Action<FrameworkThread> OnThreadRemoved = null!;

    private double updatePerSecond = 60;
    private double framesPerSecond = 60;
    private ExecutionMode executionMode;
    private bool executionModeChanged = true;
    private readonly List<FrameworkThread> threads = new();
    internal readonly WindowThread Window;
    internal readonly UpdateThread Update = new MainUpdateThread();
    internal readonly RenderThread Render = new MainRenderThread();

    /// <summary>
    /// Gets or sets how threads will be ran.
    /// </summary>
    public ExecutionMode ExecutionMode
    {
        get => executionMode;
        set
        {
            if (executionMode == value)
                return;

            executionMode = value;
            executionModeChanged = true;
        }
    }

    /// <summary>
    /// Gets or sets how often the main thread and update threads will run.
    /// </summary>
    public double UpdatePerSecond
    {
        get => updatePerSecond;
        set
        {
            if (updatePerSecond == value)
                return;

            updatePerSecond = value;

            lock (threads)
            {
                foreach (var t in threads.OfType<UpdateThread>())
                    t.UpdatePerSecond = updatePerSecond;
            }
        }
    }

    /// <summary>
    /// Gets or sets how often render threads will run.
    /// </summary>
    public double FramesPerSecond
    {
        get => framesPerSecond;
        set
        {
            if (framesPerSecond == value)
                return;

            lock (threads)
            {
                foreach (var t in threads.OfType<RenderThread>())
                    t.UpdatePerSecond = framesPerSecond;
            }

            framesPerSecond = value;
        }
    }

    public ThreadController()
    {
        AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
        TaskScheduler.UnobservedTaskException += onUnobservedTaskException;

        Window = new(run);
        Window.OnUnhandledException += onUnhandledException;

        Add(Render);
        Add(Update);
    }

    /// <summary>
    /// Adds and starts thread to this manager.
    /// </summary>
    public void Add(FrameworkThread thread)
    {
        lock (threads)
        {
            if (threads.Contains(thread))
                return;

            threads.Add(thread);
        }

        thread.UpdatePerSecond = UpdatePerSecond;
        thread.OnUnhandledException += onUnhandledException;

        if (IsRunning && ExecutionMode == ExecutionMode.MultiThread)
            thread.Start();

        OnThreadAdded?.Invoke(thread);
    }

    /// <summary>
    /// Removes and stops a thread from this manager.
    /// </summary>
    public void Remove(FrameworkThread thread)
    {
        lock (threads)
        {
            if (!threads.Contains(thread))
                return;

            threads.Remove(thread);
        }

        thread.OnUnhandledException -= onUnhandledException;

        if (IsRunning && ExecutionMode == ExecutionMode.MultiThread)
            thread.Stop();

        OnThreadRemoved?.Invoke(thread);
    }

    /// <summary>
    /// Clears all threads from this manager.
    /// </summary>
    public void Clear()
    {
        foreach (var t in threads)
        {
            Remove(t);
        }
    }

    /// <summary>
    /// Starts the threading manager.
    /// </summary>
    internal void Run()
    {
        if (IsRunning)
            return;

        Update.Post(Game.Current.Start);
        Window.Run();
    }

    /// <summary>
    /// Stops the threading manager.
    /// </summary>
    internal void Stop()
    {
        if (!IsRunning)
            return;

        Window.Stop();

        lock (threads)
        {
            foreach (var t in threads)
                t.Stop();
        }
    }

    private void onUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        Logger.Error(@"An unhandled exception has occured.", exception);

        if (AbortOnUnhandledException)
            abortFromException(sender, exception, args.IsTerminating);
    }

    private void onUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Logger.Error(@"An unobserved exception has occured.", args.Exception);

        if (AbortOnUnobservedException)
            abortFromException(sender, args.Exception, false);
    }

    private void abortFromException(object? sender, Exception exception, bool isTerminating)
    {
        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        using var reset = new ManualResetEventSlim(false);
        var dispatch = ExceptionDispatchInfo.Capture(exception);

        Window.Post(() =>
        {
            try
            {
                dispatch.Throw();
            }
            finally
            {
                reset.Set();
            }
        });

        if (isTerminating || sender == Window || ExecutionMode == ExecutionMode.SingleThread)
            return;

        reset.Wait(10000);
    }

    private void run()
    {
        ensureExecutionMode();

        if (ExecutionMode == ExecutionMode.SingleThread)
        {
            lock (threads)
            {
                foreach (var t in threads)
                    t.RunSingleFrame();
            }
        }
    }

    private void ensureExecutionMode()
    {
        if (!executionModeChanged)
            return;

        lock (threads)
        {
            foreach (var t in threads)
            {
                t.Stop();

                if (ExecutionMode == ExecutionMode.MultiThread)
                    t.Start();
            }
        }

        executionModeChanged = false;
    }

    protected override void Destroy()
    {
        Stop();

        Window.Dispose();

        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        lock (threads)
        {
            foreach (var t in threads)
                t.Dispose();
        }

        Game.Current.Dispose();

        threads.Clear();
    }

    internal class WindowThread : GameThread
    {
        private readonly IView view;
        private readonly Action run;

        public WindowThread(Action run)
            : base(@"Main")
        {
            view = Game.Current.Services.Resolve<IView>();
            this.run = run;
        }

        protected sealed override void OnNewFrame()
        {
            view.DoEvents();
            run.Invoke();
        }

        protected sealed override void Destroy()
        {
            view.Dispose();
        }
    }

    private class MainUpdateThread : UpdateThread
    {
        private readonly SystemCollection<GameSystem> systems;

        public MainUpdateThread()
            : base(@"Main")
        {
            systems = Game.Current.Services.Resolve<SystemCollection<GameSystem>>();
        }

        protected override void Update(double delta)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                var system = systems[i];

                if (!system.Enabled || system.IsDisposed || system is not IUpdateable updateable)
                    continue;

                updateable.Update(delta);
            }
        }
    }

    private class MainRenderThread : RenderThread
    {
        private readonly SystemCollection<GameSystem> systems;
        private readonly IGraphicsDevice device;
        private readonly ICommandQueue commands;

        public MainRenderThread()
            : base(@"Main")
        {
            device = Game.Current.Services.Resolve<IGraphicsDevice>();
            systems = Game.Current.Services.Resolve<SystemCollection<GameSystem>>();
            commands = device.Factory.CreateCommandQueue();
        }

        protected override void Render()
        {
            try
            {
                commands.Begin();
                commands.SetFramebuffer(device.SwapChain.Framebuffer);
                commands.Clear(0, new ClearInfo(new Color4(0, 0, 0, 1f)));
                commands.End();
                device.Submit(commands);

                for (int i = 0; i < systems.Count; i++)
                {
                    var system = systems[i];

                    if (!system.Enabled || system.IsDisposed || system is not IRenderable renderable)
                        continue;

                    renderable.Render();
                }
            }
            finally
            {
                device.SwapBuffers();
            }
        }

        protected override void Destroy()
        {
            device.Dispose();
        }
    }
}
