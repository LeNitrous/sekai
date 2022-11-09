// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Scenes;
using Sekai.Services;
using Sekai.Storage;
using Sekai.Threading;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : FrameworkObject, IGame
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
    public ServiceContainer Services { get; } = new ServiceContainer();

    /// <summary>
    /// The game window.
    /// </summary>
    public IView Window { get; private set; } = null!;

    /// <summary>
    /// The audio context.
    /// </summary>
    public AudioContext Audio { get; private set; } = null!;

    /// <summary>
    /// The input context.
    /// </summary>
    public IInputContext Input { get; private set; } = null!;

    /// <summary>
    /// Game options provided in launching the game.
    /// </summary>
    public GameOptions Options { get; private set; } = null!;

    /// <summary>
    /// The graphics context.
    /// </summary>
    public GraphicsContext Graphics { get; private set; } = null!;

    /// <summary>
    /// The virtual file system for this game.
    /// </summary>
    public VirtualStorage Storage { get; private set; } = null!;

    /// <summary>
    /// The game scene controller.
    /// </summary>
    public SceneController Scenes { get; private set; } = null!;

    /// <summary>
    /// Called when the game has loaded.
    /// </summary>
    public event Action OnLoaded = null!;

    /// <summary>
    /// Called as the game is being closed.
    /// </summary>
    public event Action OnExiting = null!;

    private bool hasStarted = false;
    private ThreadController threads = null!;

    /// <summary>
    /// Called as the game starts.
    /// </summary>
    protected virtual void Load()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void Render()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void Update()
    {
    }

    /// <summary>
    /// Called (possibly multiple times) every frame.
    /// </summary>
    protected virtual void FixedUpdate()
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
        Audio = Resolve<AudioContext>(false);
        Window = Resolve<IView>();
        Scenes = Resolve<SceneController>();
        Options = Resolve<GameOptions>();
        Storage = Resolve<VirtualStorage>();
        Graphics = Resolve<GraphicsContext>();

        threads = new(new GameWindowThread(), new GameUpdateThread(), new GameRenderThread())
        {
            ExecutionMode = Options.ExecutionMode,
            UpdatePerSecond = Options.UpdatePerSecond,
        };

        Services.Register(threads);
        hasStarted = true;

        Load();
        OnLoaded?.Invoke();

        if (!Options.Variables.Contains("SEKAI_HEADLESS_TEST"))
            threads.OnTick += showWindowOnFirstTick;

        threads.Run();

        void showWindowOnFirstTick()
        {
            if (Window is IWindow window)
            {
                window.Visible = true;
            }

            threads.OnTick -= showWindowOnFirstTick;
        }
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Dispose();
    }

    void IGame.Render()
    {
        if (!hasStarted || Graphics is null)
            return;

        Graphics.Prepare();

        try
        {
            Render();
            Services.Render();
        }
        finally
        {
            Graphics.Present();
        }
    }

    void IGame.Update(double elapsed)
    {
        if (!hasStarted)
            return;

        Update();
        Services.Update(elapsed);
    }

    void IGame.FixedUpdate()
    {
        if (!hasStarted)
            return;

        FixedUpdate();
        Services.FixedUpdate();
    }

    protected sealed override void Destroy()
    {
        if (!hasStarted)
            return;

        hasStarted = false;

        OnExiting?.Invoke();

        threads.Stop();

        Unload();

        Services.Dispose();
        Graphics.Dispose();
        Storage.Dispose();
        Window.Dispose();
        Input?.Dispose();
        Audio?.Dispose();

        current = null;
    }

    private class GameWindowThread : WindowThread
    {
        private readonly IView view = Resolve<IView>(false);
        public override void Process() => view?.DoEvents();
    }

    private class GameRenderThread : RenderThread
    {
        public override void Render() => ((IGame)Current).Render();
    }

    private class GameUpdateThread : UpdateThread
    {
        public override void FixedUpdate() => ((IGame)Current).FixedUpdate();
        public override void Update(double elapsed) => ((IGame)Current).Update(elapsed);
    }
}
