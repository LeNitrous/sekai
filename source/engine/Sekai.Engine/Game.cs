// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Engine.Assets;
using Sekai.Framework;
using Sekai.Framework.Audio;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;
using Sekai.Framework.Storage;
using Sekai.Framework.Threading;
using Sekai.Framework.Windowing;

namespace Sekai.Engine;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : FrameworkObject, IRenderable, IUpdateable
{
    /// <summary>
    /// The current game instance.
    /// </summary>
    public static Game Current
    {
        get
        {
            if (current is null)
                throw new InvalidOperationException(@"There is no active game instance present.");

            return current;
        }
    }

    private static Game? current = null;

    /// <summary>
    /// Prepares a game for running.
    /// </summary>
    public static GameBuilder<T> Setup<T>(GameOptions? options = null)
        where T : Game, new()
    {
        return new GameBuilder<T>((T)(current = new T()), options);
    }

    /// <summary>
    /// Resolves a registered game service.
    /// </summary>
    public static T Resolve<T>(bool required = true)
    {
        return Current.Services.Resolve<T>(required);
    }

    /// <summary>
    /// The services associated with this game.
    /// </summary>
    public Container Services { get; } = new Container();

    /// <summary>
    /// The game asset loader.
    /// </summary>
    public AssetLoader Assets { get; private set; } = null!;

    /// <summary>
    /// The game window.
    /// </summary>
    public IView Window { get; private set; } = null!;

    /// <summary>
    /// The audio context.
    /// </summary>
    public IAudioContext Audio { get; private set; } = null!;

    /// <summary>
    /// The input context.
    /// </summary>
    public IInputContext Input { get; private set; } = null!;

    /// <summary>
    /// Game options provided in launching the game.
    /// </summary>
    public GameOptions Options { get; private set; } = null!;

    /// <summary>
    /// The graphics device.
    /// </summary>
    public IGraphicsDevice Graphics { get; private set; } = null!;

    /// <summary>
    /// The virtual file system for this game.
    /// </summary>
    public VirtualStorage Storage { get; private set; } = null!;

    /// <summary>
    /// The game scene controller.
    /// </summary>
    public SceneController Scenes { get; private set; } = null!;

    /// <summary>
    /// The game systems associated with the game.
    /// </summary>
    public SystemCollection<GameSystem> Systems { get; private set; } = null!;

    /// <summary>
    /// Called when the game has loaded.
    /// </summary>
    public event Action OnLoaded = null!;

    /// <summary>
    /// Called as the game is being closed.
    /// </summary>
    public event Action OnExiting = null!;

    private bool hasStarted = false;
    private ICommandQueue commands = null!;
    private ThreadController threads = null!;

    /// <summary>
    /// Called as the game starts.
    /// </summary>
    protected virtual void Load()
    {
    }

    /// <summary>
    /// Called on the render thread every tick.
    /// </summary>
    protected virtual void Render()
    {
    }

    /// <summary>
    /// Called on the update thread every tick.
    /// </summary>
    protected virtual void Update()
    {
    }

    /// <summary>
    /// Called before the game exits.
    /// </summary>
    protected virtual void Unload()
    {
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Run()
    {
        if (hasStarted)
            return;

        Input = Resolve<IInputContext>(false);
        Audio = Resolve<IAudioContext>(false);
        Window = Resolve<IView>();
        Assets = Resolve<AssetLoader>();
        Scenes = Resolve<SceneController>();
        Options = Resolve<GameOptions>();
        Storage = Resolve<VirtualStorage>();
        Systems = Resolve<SystemCollection<GameSystem>>();
        Graphics = Resolve<IGraphicsDevice>();

        threads = new(new GameWindowThread())
        {
            ExecutionMode = Options.ExecutionMode,
            UpdatePerSecond = Options.UpdatePerSecond,
        };

        threads.Add(new GameUpdateThread());
        threads.Add(new GameRenderThread());
        threads.OnTick += showWindow;

        Services.Cache(threads);

        commands = Graphics.Factory.CreateCommandQueue();

        hasStarted = true;

        Load();
        OnLoaded?.Invoke();

        threads.Run();

        void showWindow()
        {
            if (Window is IWindow window)
                window.Visible = true;

            threads.OnTick -= showWindow;
        }
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Dispose();
    }

    void IRenderable.Render()
    {
        if (!hasStarted)
            return;

        try
        {
            commands.Begin();
            commands.SetFramebuffer(Graphics.SwapChain.Framebuffer);
            commands.Clear(0, new ClearInfo(new Color4(0, 0, 0, 1f)));
            commands.End();
            Graphics.Submit(commands);

            Render();
            Systems.Render();
        }
        finally
        {
            Graphics.SwapBuffers();
        }
    }

    void IUpdateable.Update()
    {
        if (!hasStarted)
            return;

        Update();
        Systems.Update();
    }

    protected sealed override void Destroy()
    {
        if (!hasStarted)
            return;

        OnExiting?.Invoke();

        threads.Stop();

        Unload();

        Systems.Dispose();
        Storage.Dispose();

        commands.Dispose();
        Graphics.WaitForIdle();
        Graphics.Dispose();

        Input?.Dispose();
        Audio?.Dispose();
        Window.Dispose();
        Services.Dispose();

        hasStarted = false;
        current = null;
    }

    private class GameWindowThread : WindowThread
    {
        private readonly IView view = Resolve<IView>();

        public override void Process() => view.DoEvents();
    }

    private class GameRenderThread : RenderThread
    {
        public GameRenderThread()
            : base("Main")
        {
        }

        public override void Process() => ((IRenderable)Current).Render();
    }

    private class GameUpdateThread : UpdateThread
    {
        public GameUpdateThread()
            : base("Main")
        {
        }

        public override void Process() => ((IUpdateable)Current).Update();
    }
}
