// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sekai.Allocation;
using Sekai.Assets;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Storages;
using Sekai.Threading;
using Sekai.Windowing;

namespace Sekai;

internal class HostBuilder : IHostBuilder
{
    public IServiceCollection Services => services;

    private Surface? surface;
    private AudioContext? audio;
    private InputContext? input;
    private Action<IHost>? onInitAction;
    private Action<IHost>? onExitAction;
    private Func<Surface, GraphicsSystem>? graphicsFactory;
    private readonly Func<Game> creator;
    private readonly GameOptions options;
    private readonly ServiceCollection services;
    private readonly VirtualStorage storage = new();
    private readonly List<IAssetLoader> assetLoaders = new();
    private readonly List<IReadOnlyList<string>> assetLoaderExtensions = new();
    private readonly Dictionary<Type, InputSystem> inputs = new();
    private readonly Dictionary<string, WorkerThread> threads = new();

    public HostBuilder(Func<Game> creator, GameOptions options, ServiceCollection services)
    {
        this.options = options;
        this.creator = creator;
        this.services = services;
    }

    public IHostBuilder UseInput<T>()
        where T : InputSystem, new()
    {
        return UseInput(new T());
    }

    public IHostBuilder UseInput(InputSystem system)
    {
        var type = system.GetType();

        if (inputs.ContainsKey(type))
            throw new ArgumentException($"Input system of type {type} is already in use.", nameof(system));

        inputs.Add(type, system);

        return this;
    }

    public IHostBuilder UseAudio<T>()
        where T : AudioSystem, new()
    {
        return UseAudio(new T());
    }

    public IHostBuilder UseAudio(AudioSystem system)
    {
        audio = new AudioContext(system);
        return this;
    }

    public IHostBuilder UseLoader<T, U>(params string[] extensions)
        where T : notnull, IAsset
        where U : class, IAssetLoader<T>, new()
    {
        return UseLoader(new U(), extensions);
    }

    public IHostBuilder UseLoader<T>(IAssetLoader<T> loader, params string[] extensions)
        where T : notnull, IAsset
    {
        assetLoaders.Add(loader);
        assetLoaderExtensions.Add(extensions.Select(static ext => ext[0] == '.' ? ext : '.' + ext).ToArray());
        return this;
    }

    public IHostBuilder UseStorage(Uri uri, Storage storage)
    {
        this.storage.Mount(uri, storage);
        return this;
    }

    public IHostBuilder UseStorage(string path, Storage storage)
    {
        this.storage.Mount(path, storage);
        return this;
    }

    public IHostBuilder UseSurface<T>()
        where T : Surface, new()
    {
        return UseSurface(new T());
    }

    public IHostBuilder UseSurface(Surface surface)
    {
        this.surface = surface;
        return this;
    }

    public IHostBuilder UseGraphics(Func<Surface, GraphicsSystem> creator)
    {
        graphicsFactory = creator;
        return this;
    }

    public IHostBuilder UseThread(string name, Action? onInit = null, Action? onLoop = null, Action? onExit = null)
    {
        if (reservedThreadNames.Contains(name, StringComparer.InvariantCultureIgnoreCase))
            throw new ArgumentException(@$"The thread name ""{name}"" is reserved.", nameof(name));

        if (!threads.ContainsKey(name))
            throw new ArgumentException(@$"A thread named ""{name}"" is already in use.", nameof(name));

        var thread = new WorkerThread(name);
        thread.OnExit += onExit;
        thread.OnStart += onInit;
        thread.OnNewFrame += onLoop;

        threads.Add(name, thread);

        return this;
    }

    public IHostBuilder UseInit(Action<IHost> action)
    {
        onInitAction += action;
        return this;
    }

    public IHostBuilder UseExit(Action<IHost> action)
    {
        onExitAction += action;
        return this;
    }

    public IHostStartup Build()
    {
        if (surface is null)
            throw new InvalidOperationException("A surface needs to be registered before building the host.");

        if (audio is null)
            throw new InvalidOperationException("An audio system needs to be registered before building the host.");

        if (graphicsFactory is null)
            throw new InvalidOperationException("A graphics system needs to be registered before building the host.");

        storage.Mount(new Uri("file:///"), new NativeStorage(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).GetStorage(options.Name));
        storage.Mount(new Uri("file:///engine/"), new AssemblyStorage(typeof(HostBuilder).Assembly).GetStorage("./Resources/"));

        var sinks = new List<LogSink>
        {
            new LogSinkConsole { Filter = m => !RuntimeInfo.IsDebug },
            new LogSinkStream(storage.Open("file:///logs.txt", FileMode.Create, FileAccess.Write))
        };

        var logger = new Logger(sinks);

        input = new(inputs.Values);

        threads.Add("Main", new WorkerThread("Main"));
        threads.Add("Game", new WorkerThread("Game"));
        threads.Add("Audio", new WorkerThread("Audio"));

        assetLoaders.Insert(0, new TextureLoader());
        assetLoaderExtensions.Insert(0, new[] { ".png", ".jpg", ".jpeg" });

        assetLoaders.Insert(0, new WaveAudioLoader());
        assetLoaderExtensions.Insert(0, new[] { ".wav" });

        assetLoaders.Insert(0, new ShaderLoader());
        assetLoaderExtensions.Insert(0, new[] { ".sksl" });

        var graphics = new GraphicsContext(graphicsFactory(surface));
        var assets = new AssetLoader(storage, assetLoaders, assetLoaderExtensions);

        services
            .AddConstant(input)
            .AddConstant(audio)
            .AddConstant(assets)
            .AddConstant(graphics)
            .AddConstant<ILogger>(logger)
            .AddConstant<Storage>(storage)
            .AddConstant<ISurface>(surface);

        var host = new Host(creator, options, logger, surface, storage, audio, input, graphics, services, threads, onInitAction, onExitAction);

        return host;
    }

    private static readonly string[] reservedThreadNames = new[] { "main", "game", "audio" };
}
