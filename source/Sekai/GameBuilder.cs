// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Processors;
using Sekai.Rendering;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

public sealed class GameBuilder<T>
    where T : Game, new()
{
    private Lazy<Surface>? surface;
    private Lazy<AudioSystem>? audio;
    private Lazy<GraphicsSystem>? graphics;
    private Action<ServiceLocator>? configureServicesAction;
    private readonly GameOptions options;
    private readonly Queue<Action> loadActionQueue = new();
    private readonly Queue<Action> exitActionQueue = new();
    private readonly Queue<Action> buildActionQueue = new();
    private readonly List<Lazy<InputSystem>> inputs = new();

    internal GameBuilder(GameOptions? options = null)
    {
        this.options = options ?? new();
    }

    /// <summary>
    /// Uses the surface of a given type.
    /// </summary>
    public GameBuilder<T> UseSurface<U>()
        where U : Surface, new()
    {
        surface = new Lazy<Surface>(() => new U());
        return this;
    }

    /// <summary>
    /// Uses the graphics system of a given type.
    /// </summary>
    public GameBuilder<T> UseGraphics<U>()
        where U : GraphicsSystem, new()
    {
        graphics = new Lazy<GraphicsSystem>(() => new U());
        return this;
    }

    /// <summary>
    /// Uses the audio system of a given type.
    /// </summary>
    public GameBuilder<T> UseAudio<U>()
        where U : AudioSystem, new()
    {
        audio = new Lazy<AudioSystem>(() => new U());
        return this;
    }

    /// <summary>
    /// Uses the input system of a given type.
    /// </summary>
    /// <remarks>
    /// This can be called multiple times to support multiple input systems.
    /// </remarks>
    public GameBuilder<T> UseInput<U>()
        where U : InputSystem, new()
    {
        inputs.Add(new Lazy<InputSystem>(() => new U()));
        return this;
    }

    /// <summary>
    /// Registers an action for configuring services.
    /// </summary>
    public GameBuilder<T> ConfigureServices(Action<ServiceLocator> action)
    {
        configureServicesAction += action;
        return this;
    }

    /// <summary>
    /// Adds an action invoked during the game's build step.
    /// </summary>
    public GameBuilder<T> AddBuildAction(Action action)
    {
        buildActionQueue.Enqueue(action);
        return this;
    }

    /// <summary>
    /// Adds an action invoked during the game's load step.
    /// </summary>
    public GameBuilder<T> AddLoadAction(Action action)
    {
        loadActionQueue.Enqueue(action);
        return this;
    }

    /// <summary>
    /// Adds an action invoked during the game's exit step.
    /// </summary>
    public GameBuilder<T> AddExitAction(Action action)
    {
        exitActionQueue.Enqueue(action);
        return this;
    }

    /// <summary>
    /// Builds the game.
    /// </summary>
    public T Build()
    {
        ServiceLocator.Initialize();

        var factory = new LoggerFactory();

        if (RuntimeInfo.IsDebug)
            factory.Add(new LogListenerConsole());

        ServiceLocator.Current.Cache(options);
        ServiceLocator.Current.Cache(factory);
        ServiceLocator.Current.Cache(factory.GetLogger());
        ServiceLocator.Current.Cache<Time>();
        ServiceLocator.Current.Cache<ProcessorManager>();
        ServiceLocator.Current.Cache<ShaderUniformManager>();

        var storage = new StorageContext();
        storage.Mount(@"engine", new AssemblyBackedStorage(typeof(Game).Assembly, @"Resources"));
        ServiceLocator.Current.Cache(storage);

        var loader = new AssetLoader();
        var loaderRegistry = (IAssetLoaderRegistry)loader;
        loaderRegistry.Register(new TextureLoader());
        loaderRegistry.Register(new ShaderLoader());
        loaderRegistry.Register(new WaveAudioLoader());

        ServiceLocator.Current.Cache(loader);

        if (surface is null)
            throw new InvalidOperationException($"A surface was not registered during setup.");

        var surfaceValue = surface.Value;
        ServiceLocator.Current.Cache(surfaceValue);

        if (surfaceValue is IWindow window)
            ServiceLocator.Current.Cache(window);

        if (graphics is null)
            throw new InvalidOperationException($"A graphics system was not registered during setup.");

        ServiceLocator.Current.Cache(graphics.Value);
        ServiceLocator.Current.Cache(graphics.Value.CreateShaderTranspiler());
        ServiceLocator.Current.Cache<GraphicsContext>();
        ServiceLocator.Current.Cache<Renderer>();

        if (audio is null)
            throw new InvalidOperationException($"An audio system was not registered during setup.");

        ServiceLocator.Current.Cache(audio.Value);
        ServiceLocator.Current.Cache<AudioContext>();

        var inputContext = new InputContext();

        foreach (var input in inputs)
            inputContext.Attach(input.Value);

        ServiceLocator.Current.Cache(inputContext);

        var game = new T();

        ServiceLocator.Current.Cache(game);
        ServiceLocator.Current.Cache<Game>(game);

        while (loadActionQueue.TryDequeue(out var action))
            game.OnLoad += action;

        while (exitActionQueue.TryDequeue(out var action))
            game.OnExit += action;

        while (buildActionQueue.TryDequeue(out var action))
            action();

        configureServicesAction?.Invoke(ServiceLocator.Current);

        return game;
    }

    /// <summary>
    /// Builds then runs the game.
    /// </summary>
    public void Run()
    {
        Build().Run();
        ServiceLocator.Shutdown();
    }
}
