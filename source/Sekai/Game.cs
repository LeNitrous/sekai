// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Scenes;
using Sekai.Storages;
using Sekai.Threading;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : DependencyObject
{
    /// <summary>
    /// Prepares a game for running.
    /// </summary>
    public static GameBuilder<T> Setup<T>(GameOptions? options = null)
        where T : Game, new()
    {
        return new GameBuilder<T>(options);
    }

    /// <summary>
    /// Gets whether the game is loaded or not.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Called when the game has loaded.
    /// </summary>
    public event Action? OnLoad;

    /// <summary>
    /// Called as the game is being closed.
    /// </summary>
    public event Action? OnExit;

    /// <summary>
    /// Called after the game has updated a frame.
    /// </summary>
    public event Action? OnUpdate;

    /// <summary>
    /// Called after the game has rendered a frame.
    /// </summary>
    public event Action? OnRender;

    /// <inheritdoc cref="GameRunner.ExecutionMode"/>
    public ExecutionMode ExecutionMode
    {
        get => runner.ExecutionMode;
        set => runner.ExecutionMode = value;
    }

    /// <inheritdoc cref="GameRunner.UpdatePerSecond"/>
    public double UpdatePerSecond
    {
        get => runner.UpdatePerSecond;
        set => runner.UpdatePerSecond = value;
    }

    /// <inheritdoc cref="GameRunner.OnException"/>
    public event Func<Exception, bool> OnException
    {
        add => runner.OnException += value;
        remove => runner.OnException -= value;
    }

    /// <summary>
    /// The game options passed during construction.
    /// </summary>
    [Resolved]
    protected GameOptions Options { get; set; } = null!;

    /// <summary>
    /// The game's graphics context.
    /// </summary>
    [Resolved]
    protected GraphicsContext Graphics { get; set; } = null!;

    /// <summary>
    /// The game's audio context.
    /// </summary>
    [Resolved]
    protected AudioContext Audio { get; set; } = null!;

    /// <summary>
    /// The game's storage context.
    /// </summary>
    [Resolved]
    protected VirtualStorage Storage { get; set; } = null!;

    /// <summary>
    /// The game's asset loader.
    /// </summary>
    [Resolved]
    protected AssetLoader Content { get; set; } = null!;

    /// <summary>
    /// The game's input context.
    /// </summary>
    [Resolved]
    protected InputContext Input { get; set; } = null!;

    /// <summary>
    /// The game's rendering surface.
    /// </summary>
    [Resolved]
    protected Surface Surface { get; set; } = null!;

    /// <summary>
    /// The game time.
    /// </summary>
    [Resolved]
    protected Time Time { get; set; } = null!;

    /// <summary>
    /// The game's scene collection.
    /// </summary>
    protected SceneCollection Scenes { get; } = new();

    private readonly GameRunner runner = new();

    public Game()
    {
        OnUpdate += Time.Update;
        OnUpdate += Input.Update;
        OnUpdate += Update;
        OnRender += Render;
        OnUpdate += Scenes.Update;
        OnRender += Scenes.Render;
    }

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
        var update = new WorkerThread("Update", null, false);
        update.OnNewFrame += onWorkerLoop;
        update.OnStart += onWorkerInit;
        update.OnExit += onWorkerExit;

        var audio = new WorkerThread("Audio", null, false);
        audio.OnNewFrame += Audio.Update;
        audio.OnExit += Audio.Dispose;

        runner.UpdatePerSecond = Options.UpdatePerSecond;
        runner.ExecutionMode = Options.ExecutionMode;
        runner.Add(update);
        runner.Add(audio);

        if (Surface is IWindow window)
        {
            window.Visible = false;
            window.Size = Options.Size;
            window.Title = Options.Title;
            window.State = Options.State;
            window.Border = Options.Border;
        }

        Surface.OnUpdate += runner.Update;
        Surface.OnClose += Dispose;
        Surface.Run();
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Surface.Close();
        Surface.OnUpdate -= runner.Update;
        Surface.OnClose -= Dispose;
    }

    private void onWorkerInit()
    {
        Graphics.MakeCurrent();
        IsLoaded = true;
        Load();
        OnLoad?.Invoke();
    }

    private bool hasShownWindow = false;

    private void onWorkerLoop()
    {
        OnUpdate?.Invoke();
        Graphics.Prepare(Surface.Size);
        OnRender?.Invoke();
        Graphics.Present();

        if (!hasShownWindow && Surface is IWindow window)
        {
            window.Visible = true;
            hasShownWindow = true;
        }
    }

    private void onWorkerExit()
    {
        Unload();
        OnExit?.Invoke();
        IsLoaded = false;
    }

    protected override void Destroy()
    {
        OnUpdate -= Time.Update;
        OnUpdate -= Input.Update;
        OnUpdate -= Update;
        OnRender -= Render;
        OnUpdate -= Scenes.Update;
        OnRender -= Scenes.Render;
        Scenes.Dispose();
        runner.Dispose();
    }
}
