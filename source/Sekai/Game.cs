// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Scenes;
using Sekai.Storage;

namespace Sekai;

/// <summary>
/// The game application and the entry point for Sekai.
/// </summary>
public abstract class Game : FrameworkObject
{
    /// <summary>
    /// Prepares a game for running.
    /// </summary>
    public static GameBuilder<T> Setup<T>(GameOptions? options = null)
        where T : Game, new()
    {
        return new GameBuilder<T>(new T(), options);
    }

    /// <summary>
    /// Called when the game has loaded.
    /// </summary>
    public event Action? OnLoaded;

    /// <summary>
    /// Called as the game is being closed.
    /// </summary>
    public event Action? OnExiting;

    /// <summary>
    /// The game's scenes.
    /// </summary>
    public SceneCollection Scenes { get; private set; } = null!;

    /// <summary>
    /// The game's graphics context.
    /// </summary>
    public GraphicsContext Graphics { get; private set; } = null!;

    /// <summary>
    /// The game's storage context.
    /// </summary>
    public StorageContext? Storage { get; private set; }

    /// <summary>
    /// The game's audio context.
    /// </summary>
    public AudioContext? Audio { get; private set; }

    /// <summary>
    /// The game's input context.
    /// </summary>
    public InputContext? Input { get; private set; }

    private bool hasStarted;
    private GameRunner? runner;

    /// <summary>
    /// Called as the game starts.
    /// </summary>
    public virtual void Load()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    public virtual void Render()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    public virtual void Update(double elapsed)
    {
    }

    /// <summary>
    /// Called before the game exits.
    /// </summary>
    public virtual void Unload()
    {
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Run()
    {
        if (hasStarted)
            return;

        hasStarted = true;

        runner = Services.Current.Resolve<GameRunner>();
        Audio = Services.Current.Resolve<AudioContext>(false);
        Input = Services.Current.Resolve<InputContext>(false);
        Scenes = Services.Current.Resolve<SceneCollection>();
        Storage = Services.Current.Resolve<StorageContext>(false);
        Graphics = Services.Current.Resolve<GraphicsContext>();

        Load();
        OnLoaded?.Invoke();

        runner.Start();
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        runner?.Stop();
    }

    protected sealed override void Destroy()
    {
        if (!hasStarted)
            return;

        OnExiting?.Invoke();
        Unload();
    }
}
